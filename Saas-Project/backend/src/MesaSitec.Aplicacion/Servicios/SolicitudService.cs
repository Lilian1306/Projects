using MesaSitec.Aplicacion.DTOs.Comun;
using MesaSitec.Aplicacion.DTOs.Solicitudes;
using MesaSitec.Aplicacion.Interfaces;
using MesaSitec.Dominio.Entidades;
using MesaSitec.Dominio.Enums;
using MesaSitec.Dominio.Excepciones;
using Microsoft.EntityFrameworkCore;

namespace MesaSitec.Aplicacion.Servicios;

public class SolicitudService : ISolicitudService
{
    private readonly DbContext _context;

    public SolicitudService(DbContext context)
    {
        _context = context;
    }

    public async Task<ResultadoPaginadoDto<SolicitudResumenDto>> ListarSolicitudesAsync(
        Guid tenantId, Guid usuarioId, Rol rol, SolicitudFiltrosDto filtros)
    {
        if (filtros.Page < 1 || filtros.PageSize is < 1 or > 100)
        {
            throw new ReglaNegocioException("PARAMETRO_INVALIDO", "El parámetro page debe ser mayor a 0 y pageSize debe estar entre 1 y 100.", 400);
        }

        var ahora = DateTime.UtcNow;

        var query = _context.Set<Solicitud>()
            .AsNoTracking()
            .Include(s => s.Categoria)
            .Include(s => s.Agente)
            .Where(s => s.TenantId == tenantId);

        if (rol == Rol.Solicitante) query = query.Where(s => s.SolicitanteId == usuarioId);
        if (filtros.Estado.HasValue) query = query.Where(s => s.Estado == filtros.Estado.Value);
        if (filtros.Prioridad.HasValue) query = query.Where(s => s.Prioridad == filtros.Prioridad.Value);
        if (filtros.CategoriaId.HasValue) query = query.Where(s => s.CategoriaId == filtros.CategoriaId.Value);
        if (filtros.AgenteId.HasValue) query = query.Where(s => s.AgenteId == filtros.AgenteId.Value);

        if (!string.IsNullOrWhiteSpace(filtros.Q))
        {
            var qLower = filtros.Q.Trim().ToLower();
            query = query.Where(s => s.Titulo.ToLower().Contains(qLower) || 
                                     s.Descripcion.ToLower().Contains(qLower) || 
                                     s.Codigo.ToLower().Contains(qLower));
        }

        if (filtros.Vencidas == true)
        {
            query = query.Where(s => s.FechaLimiteSla < ahora && 
                                     s.Estado != Estado.Resuelta && 
                                     s.Estado != Estado.Cerrada && 
                                     s.Estado != Estado.Cancelada);
        }

        query = filtros.Sort switch
        {
            "-fechaCreacion" => query.OrderByDescending(s => s.FechaCreacion),
            "fechaCreacion"  => query.OrderBy(s => s.FechaCreacion),
            "codigo"         => query.OrderBy(s => s.Codigo),
            // Prioridad se guarda como string en la BD (HasConversion<string>), por lo que
            // OrderBy(s => s.Prioridad) ordenaria alfabeticamente (Alta,Baja,Critica,Media).
            // Se usa un peso explicito para lograr el orden semantico Baja<Media<Alta<Critica.
            "prioridad"      => query.OrderBy(s => s.Prioridad == Prioridad.Critica ? 4 :
                                                   s.Prioridad == Prioridad.Alta ? 3 :
                                                   s.Prioridad == Prioridad.Media ? 2 : 1),
            "-prioridad"     => query.OrderByDescending(s => s.Prioridad == Prioridad.Critica ? 4 :
                                                            s.Prioridad == Prioridad.Alta ? 3 :
                                                            s.Prioridad == Prioridad.Media ? 2 : 1),
            _                => query.OrderByDescending(s => s.FechaCreacion)
        };

        int total = await query.CountAsync();
        int totalPaginas = total == 0 ? 1 : (int)Math.Ceiling(total / (double)filtros.PageSize);

        var items = await query
            .Skip((filtros.Page - 1) * filtros.PageSize)
            .Take(filtros.PageSize)
            .Select(s => MapearAResumen(s, ahora))
            .ToListAsync();

        return new ResultadoPaginadoDto<SolicitudResumenDto>
        {
            Items = items,
            Page = filtros.Page,
            PageSize = filtros.PageSize,
            Total = total,
            TotalPaginas = totalPaginas
        };
    }

    public async Task<SolicitudDetalleDto> ObtenerDetalleAsync(Guid tenantId, Guid usuarioId, Rol rol, Guid solicitudId)
    {
        var sol = await _context.Set<Solicitud>()
            .AsNoTracking()
            .Include(s => s.Categoria)
            .Include(s => s.Solicitante)
            .Include(s => s.Agente)
            .FirstOrDefaultAsync(s => s.Id == solicitudId && s.TenantId == tenantId)
            ?? throw new ReglaNegocioException("RECURSO_NO_ENCONTRADO", "La solicitud no existe.", 404);

        if (rol == Rol.Solicitante && sol.SolicitanteId != usuarioId)
        {
            throw new ReglaNegocioException("RECURSO_NO_ENCONTRADO", "La solicitud no existe.", 404);
        }

        return MapearADetalle(sol, DateTime.UtcNow);
    }

    public async Task<SolicitudDetalleDto> CrearSolicitudAsync(Guid tenantId, Guid solicitanteId, CrearSolicitudDto dto)
    {
        var categoria = await ValidarYObtenerCategoriaAsync(tenantId, dto.CategoriaId);

        var ahora = DateTime.UtcNow;
        string prefijoAnio = $"SOL-{ahora.Year}-";

        int correlativoActual = await _context.Set<Solicitud>()
            .CountAsync(s => s.TenantId == tenantId && s.Codigo.StartsWith(prefijoAnio)) + 1;

        var solicitud = new Solicitud
        {
            TenantId = tenantId,
            Codigo = $"{prefijoAnio}{correlativoActual:D5}",
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            CategoriaId = dto.CategoriaId,
            Prioridad = dto.Prioridad,
            Estado = Estado.Nueva,
            SolicitanteId = solicitanteId,
            FechaCreacion = ahora
        };

        solicitud.CalcularFechaLimiteSla(categoria.SlaHoras);

        _context.Set<Solicitud>().Add(solicitud);
        await _context.SaveChangesAsync();

        return await ObtenerDetalleAsync(tenantId, solicitanteId, Rol.Admin, solicitud.Id);
    }

    public async Task<SolicitudDetalleDto> EditarSolicitudAsync(Guid tenantId, Guid usuarioId, Rol rol, Guid solicitudId, EditarSolicitudDto dto)
    {
        var sol = await _context.Set<Solicitud>()
            .FirstOrDefaultAsync(s => s.Id == solicitudId && s.TenantId == tenantId)
            ?? throw new ReglaNegocioException("RECURSO_NO_ENCONTRADO", "La solicitud no existe.", 404);

        if (rol == Rol.Solicitante && (sol.SolicitanteId != usuarioId || sol.Estado != Estado.Nueva))
        {
            throw new ReglaNegocioException("OPERACION_NO_PERMITIDA", "No tiene permisos para editar esta solicitud.", 403);
        }

        var categoria = await ValidarYObtenerCategoriaAsync(tenantId, dto.CategoriaId);

        sol.Titulo = dto.Titulo;
        sol.Descripcion = dto.Descripcion;
        sol.CategoriaId = dto.CategoriaId;
        sol.Prioridad = dto.Prioridad;
        sol.CalcularFechaLimiteSla(categoria.SlaHoras);

        await _context.SaveChangesAsync();
        return await ObtenerDetalleAsync(tenantId, usuarioId, rol, sol.Id);
    }

    public async Task<SolicitudDetalleDto> EjecutarTransicionAsync(Guid tenantId, Guid usuarioId, Rol rol, Guid solicitudId, EjecutarTransicionDto dto)
    {
        var sol = await _context.Set<Solicitud>()
            .FirstOrDefaultAsync(s => s.Id == solicitudId && s.TenantId == tenantId)
            ?? throw new ReglaNegocioException("RECURSO_NO_ENCONTRADO", "La solicitud no existe.", 404);

        string accion = dto.Accion.Trim().ToLower();

        ValidarPermisoTransicion(rol, sol.SolicitanteId, usuarioId, accion);

        if (accion == "asignar")
        {
            var agente = await ValidarYObtenerAgenteAsync(tenantId, dto.AgenteId);
            sol.Asignar(agente.Id);
        }
        else
        {
            var acciones = new Dictionary<string, Action>
            {
                ["iniciar"]  = () => sol.Iniciar(),
                ["resolver"] = () => sol.Resolver(dto.Motivo ?? string.Empty, DateTime.UtcNow),
                ["cerrar"]   = () => sol.Cerrar(),
                ["reabrir"]  = () => sol.Reabrir(),
                ["cancelar"] = () => sol.Cancelar(dto.Motivo ?? string.Empty)
            };

            if (!acciones.TryGetValue(accion, out var ejecutar))
            {
                throw new ReglaNegocioException("TRANSICION_INVALIDA", $"Acción '{dto.Accion}' no reconocida.", 409);
            }

            ejecutar();
        }

        await _context.SaveChangesAsync();
        return await ObtenerDetalleAsync(tenantId, usuarioId, rol, sol.Id);
    }

    #region Helpers Privados

    internal static void ValidarPermisoTransicion(Rol rol, Guid solicitanteId, Guid usuarioId, string accion)
    {
        var esPermitido = accion switch
        {
            "asignar" or "iniciar" or "resolver" or "reabrir" => rol != Rol.Solicitante,
            "cerrar"   => rol != Rol.Solicitante || solicitanteId == usuarioId,
            "cancelar" => rol == Rol.Admin,
            _ => throw new ReglaNegocioException("TRANSICION_INVALIDA", $"Acción '{accion}' no reconocida.", 409)
        };

        if (!esPermitido)
        {
            throw new ReglaNegocioException("OPERACION_NO_PERMITIDA", $"El rol {rol} no tiene permisos para '{accion}'.", 403);
        }
    }

    private async Task<Categoria> ValidarYObtenerCategoriaAsync(Guid tenantId, Guid categoriaId) =>
        await _context.Set<Categoria>()
            .FirstOrDefaultAsync(c => c.Id == categoriaId && c.TenantId == tenantId && c.Activo)
        ?? throw new ReglaNegocioException("VALIDACION", "La categoría especificada no es válida.", 422);

    private async Task<Usuario> ValidarYObtenerAgenteAsync(Guid tenantId, Guid? agenteId)
    {
        if (!agenteId.HasValue) throw new ReglaNegocioException("AGENTE_INVALIDO", "Debe especificar un agente válido.", 422);

        var agente = await _context.Set<Usuario>()
            .FirstOrDefaultAsync(u => u.Id == agenteId.Value && u.TenantId == tenantId && u.Activo);

        if (agente == null || (agente.Rol != Rol.Agente && agente.Rol != Rol.Admin))
        {
            throw new ReglaNegocioException("AGENTE_INVALIDO", "El agente seleccionado no pertenece a su organización o no tiene el rol adecuado.", 422);
        }

        return agente;
    }

    private static SolicitudResumenDto MapearAResumen(Solicitud s, DateTime fechaComparacion) => new()
    {
        Id = s.Id,
        Codigo = s.Codigo,
        Titulo = s.Titulo,
        Estado = s.Estado.ToString(),
        Prioridad = s.Prioridad.ToString(),
        Categoria = new ResumenRefDto { Id = s.CategoriaId, Nombre = s.Categoria?.Nombre ?? string.Empty },
        Agente = s.Agente != null ? new ResumenRefDto { Id = s.Agente.Id, Nombre = s.Agente.Nombre } : null,
        FechaCreacion = s.FechaCreacion,
        FechaLimiteSla = s.FechaLimiteSla,
        Vencida = s.EsVencida(fechaComparacion)
    };

    private static SolicitudDetalleDto MapearADetalle(Solicitud s, DateTime fechaComparacion) => new()
    {
        Id = s.Id,
        Codigo = s.Codigo,
        Titulo = s.Titulo,
        Descripcion = s.Descripcion,
        Estado = s.Estado.ToString(),
        Prioridad = s.Prioridad.ToString(),
        Categoria = new ResumenRefDto { Id = s.CategoriaId, Nombre = s.Categoria?.Nombre ?? string.Empty },
        Solicitante = new ResumenRefDto { Id = s.SolicitanteId, Nombre = s.Solicitante?.Nombre ?? string.Empty },
        Agente = s.Agente != null ? new ResumenRefDto { Id = s.Agente.Id, Nombre = s.Agente.Nombre } : null,
        FechaCreacion = s.FechaCreacion,
        FechaLimiteSla = s.FechaLimiteSla,
        Vencida = s.EsVencida(fechaComparacion),
        MotivoResolucion = s.MotivoResolucion,
        MotivoCancelacion = s.MotivoCancelacion,
        FechaResolucion = s.FechaResolucion
    };

    #endregion
}