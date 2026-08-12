using MesaSitec.Aplicacion.DTOs.Categorias;
using MesaSitec.Aplicacion.Interfaces;
using MesaSitec.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace MesaSitec.Aplicacion.Servicios;

public class CategoriaService : ICategoriaService
{
    private readonly DbContext _context;

    public CategoriaService(DbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoriaDto>> ObtenerCategoriasActivasAsync(Guid tenantId)
    {
        return await _context.Set<Categoria>()
            .AsNoTracking()
            .Where(c => c.TenantId == tenantId && c.Activo)
            .Select(c => new CategoriaDto
            {
                Id = c.Id,
                Nombre = c.Nombre,
                SlaHoras = c.SlaHoras
            })
            .ToListAsync();
    }
}
