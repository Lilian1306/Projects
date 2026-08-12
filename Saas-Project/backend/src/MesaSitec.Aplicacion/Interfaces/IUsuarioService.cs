using MesaSitec.Aplicacion.DTOs.Usuarios;

namespace MesaSitec.Aplicacion.Interfaces;

public interface IUsuarioService
{
    Task<IEnumerable<AgenteDto>> ObtenerAgentesTenantAsync(Guid tenantId);
}
