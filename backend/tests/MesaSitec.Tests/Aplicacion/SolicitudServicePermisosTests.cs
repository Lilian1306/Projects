using System.Linq;
using MesaSitec.Aplicacion.DTOs.Solicitudes;
using MesaSitec.Aplicacion.Servicios;
using MesaSitec.Dominio.Entidades;
using MesaSitec.Dominio.Enums;
using MesaSitec.Dominio.Excepciones;
using MesaSitec.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MesaSitec.Tests.Aplicacion;

// RN-03: aislamiento de datos por rol (Listar/Detalle/Editar), usando EF Core InMemory
// para ejercitar el servicio real sin levantar la aplicación completa.
public class SolicitudServicePermisosTests
{
    private sealed record Escenario(
        MesaSitecDbContext Ctx,
        Guid TenantId,
        Guid CategoriaId,
        Guid SolicitanteAId,
        Guid SolicitanteBId,
        Guid SolicitudDeAId,
        Guid SolicitudDeBId);

    private static Escenario SembrarEscenario()
    {
        var options = new DbContextOptionsBuilder<MesaSitecDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new MesaSitecDbContext(options);

        var tenantId = Guid.NewGuid();
        var categoriaId = Guid.NewGuid();
        var solicitanteAId = Guid.NewGuid();
        var solicitanteBId = Guid.NewGuid();

        ctx.Tenants.Add(new Tenant { Id = tenantId, Nombre = "Tenant Test", Activo = true });
        ctx.Categorias.Add(new Categoria { Id = categoriaId, TenantId = tenantId, Nombre = "Incidente", SlaHoras = 8, Activo = true });
        ctx.Usuarios.Add(new Usuario { Id = solicitanteAId, TenantId = tenantId, Email = "a@test.com", Nombre = "Solicitante A", Rol = Rol.Solicitante, Activo = true });
        ctx.Usuarios.Add(new Usuario { Id = solicitanteBId, TenantId = tenantId, Email = "b@test.com", Nombre = "Solicitante B", Rol = Rol.Solicitante, Activo = true });

        var solicitudDeA = new Solicitud
        {
            TenantId = tenantId,
            Codigo = "SOL-2026-00001",
            Titulo = "Solicitud de A",
            Descripcion = "Descripción de prueba con longitud suficiente.",
            CategoriaId = categoriaId,
            Prioridad = Prioridad.Media,
            Estado = Estado.Nueva,
            SolicitanteId = solicitanteAId,
            FechaCreacion = DateTime.UtcNow
        };
        solicitudDeA.CalcularFechaLimiteSla(8);

        var solicitudDeB = new Solicitud
        {
            TenantId = tenantId,
            Codigo = "SOL-2026-00002",
            Titulo = "Solicitud de B",
            Descripcion = "Descripción de prueba con longitud suficiente.",
            CategoriaId = categoriaId,
            Prioridad = Prioridad.Media,
            Estado = Estado.Nueva,
            SolicitanteId = solicitanteBId,
            FechaCreacion = DateTime.UtcNow
        };
        solicitudDeB.CalcularFechaLimiteSla(8);

        ctx.Solicitudes.AddRange(solicitudDeA, solicitudDeB);
        ctx.SaveChanges();

        return new Escenario(ctx, tenantId, categoriaId, solicitanteAId, solicitanteBId, solicitudDeA.Id, solicitudDeB.Id);
    }

    [Fact]
    public async Task Listar_ComoSolicitante_SoloDevuelveLasSolicitudesPropias()
    {
        var e = SembrarEscenario();
        var service = new SolicitudService(e.Ctx);

        var resultado = await service.ListarSolicitudesAsync(e.TenantId, e.SolicitanteAId, Rol.Solicitante, new SolicitudFiltrosDto());

        Assert.Single(resultado.Items);
        Assert.Equal("SOL-2026-00001", resultado.Items.First().Codigo);
    }

    [Fact]
    public async Task Listar_ComoAdmin_DevuelveTodasLasSolicitudesDelTenant()
    {
        var e = SembrarEscenario();
        var service = new SolicitudService(e.Ctx);

        var resultado = await service.ListarSolicitudesAsync(e.TenantId, Guid.NewGuid(), Rol.Admin, new SolicitudFiltrosDto());

        Assert.Equal(2, resultado.Items.Count());
    }

    [Fact]
    public async Task ObtenerDetalle_ComoSolicitante_DeUnaSolicitudAjena_LanzaRecursoNoEncontrado()
    {
        var e = SembrarEscenario();
        var service = new SolicitudService(e.Ctx);

        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() =>
            service.ObtenerDetalleAsync(e.TenantId, e.SolicitanteAId, Rol.Solicitante, e.SolicitudDeBId));

        Assert.Equal("RECURSO_NO_ENCONTRADO", ex.Codigo);
        Assert.Equal(404, ex.StatusCode);
    }

    [Fact]
    public async Task ObtenerDetalle_ComoSolicitante_DeSuPropiaSolicitud_NoLanzaExcepcion()
    {
        var e = SembrarEscenario();
        var service = new SolicitudService(e.Ctx);

        var detalle = await service.ObtenerDetalleAsync(e.TenantId, e.SolicitanteAId, Rol.Solicitante, e.SolicitudDeAId);

        Assert.Equal("SOL-2026-00001", detalle.Codigo);
    }

    [Fact]
    public async Task EditarSolicitud_ComoSolicitante_SobreSolicitudDeOtroUsuario_LanzaOperacionNoPermitida()
    {
        var e = SembrarEscenario();
        var service = new SolicitudService(e.Ctx);
        var dto = new EditarSolicitudDto
        {
            Titulo = "Nuevo título válido",
            Descripcion = "Nueva descripción con longitud suficiente.",
            CategoriaId = e.CategoriaId,
            Prioridad = Prioridad.Alta
        };

        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() =>
            service.EditarSolicitudAsync(e.TenantId, e.SolicitanteAId, Rol.Solicitante, e.SolicitudDeBId, dto));

        Assert.Equal("OPERACION_NO_PERMITIDA", ex.Codigo);
        Assert.Equal(403, ex.StatusCode);
    }

    [Fact]
    public async Task EditarSolicitud_ComoSolicitante_SobrePropiaEnEstadoDistintoDeNueva_LanzaOperacionNoPermitida()
    {
        var e = SembrarEscenario();

        var solicitud = await e.Ctx.Solicitudes.FirstAsync(s => s.Id == e.SolicitudDeAId);
        solicitud.Estado = Estado.Asignada;
        await e.Ctx.SaveChangesAsync();

        var service = new SolicitudService(e.Ctx);
        var dto = new EditarSolicitudDto
        {
            Titulo = "Nuevo título válido",
            Descripcion = "Nueva descripción con longitud suficiente.",
            CategoriaId = e.CategoriaId,
            Prioridad = Prioridad.Alta
        };

        var ex = await Assert.ThrowsAsync<ReglaNegocioException>(() =>
            service.EditarSolicitudAsync(e.TenantId, e.SolicitanteAId, Rol.Solicitante, e.SolicitudDeAId, dto));

        Assert.Equal("OPERACION_NO_PERMITIDA", ex.Codigo);
    }
}
