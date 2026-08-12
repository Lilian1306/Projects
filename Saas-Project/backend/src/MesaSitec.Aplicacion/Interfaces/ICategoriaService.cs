using MesaSitec.Aplicacion.DTOs.Categorias;

namespace MesaSitec.Aplicacion.Interfaces;

public interface ICategoriaService
{
    Task<IEnumerable<CategoriaDto>> ObtenerCategoriasActivasAsync(Guid tenantId);
}
