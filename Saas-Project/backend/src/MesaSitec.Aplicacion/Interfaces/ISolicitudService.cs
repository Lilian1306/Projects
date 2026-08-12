using MesaSitec.Aplicacion.DTOs.Solicitudes;
using MesaSitec.Dominio.Enums;

namespace MesaSitec.Aplicacion.Interfaces;

public interface ISolicitudService
{
    Task<ResultadoPaginadoDto<SolicitudResumenDto>> ListarSolicitudesAsync(Guid tenantId, Guid usuarioId, Rol rol, SolicitudFiltrosDto filtros);
    Task<SolicitudDetalleDto> ObtenerDetalleAsync(Guid tenantId, Guid usuarioId, Rol rol, Guid solicitudId);
    Task<SolicitudDetalleDto> CrearSolicitudAsync(Guid tenantId, Guid solicitanteId, CrearSolicitudDto dto);
    Task<SolicitudDetalleDto> EditarSolicitudAsync(Guid tenantId, Guid usuarioId, Rol rol, Guid solicitudId, EditarSolicitudDto dto);
    Task<SolicitudDetalleDto> EjecutarTransicionAsync(Guid tenantId, Guid usuarioId, Rol rol, Guid solicitudId, EjecutarTransicionDto dto);
}
