using MesaSitec.Aplicacion.DTOs.Usuarios;
using MesaSitec.Aplicacion.Interfaces;
using MesaSitec.Dominio.Entidades;
using MesaSitec.Dominio.Enums;
using Microsoft.EntityFrameworkCore;

namespace MesaSitec.Aplicacion.Servicios;

public class UsuarioService : IUsuarioService
{
    private readonly DbContext _context;

    public UsuarioService(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AgenteDto>> ObtenerAgentesTenantAsync(Guid tenantId)
    {
        return await _context.Set<Usuario>()
            .AsNoTracking()
            .Where(u => u.TenantId == tenantId && u.Activo && (u.Rol == Rol.Agente || u.Rol == Rol.Admin))
            .OrderBy(u => u.Nombre)
            .Select(u => new AgenteDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Email = u.Email
            })
            .ToListAsync();
    }
}
