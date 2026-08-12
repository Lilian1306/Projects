using MesaSitec.Aplicacion.DTOs.Solicitudes;
using MesaSitec.Aplicacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesaSitec.Api.Controllers;

[Authorize]
public class SolicitudesController : BaseApiController
{
    private readonly ISolicitudService _solicitudService;

    public SolicitudesController(ISolicitudService solicitudService)
    {
        _solicitudService = solicitudService;
    }

    [HttpGet]
    public async Task<ActionResult<ResultadoPaginadoDto<SolicitudResumenDto>>> Listar([FromQuery] SolicitudFiltrosDto filtros)
    {
        var tenantId = ObtenerTenantId();
        var usuarioId = ObtenerUsuarioId();
        var rol = ObtenerRol();

        var resultado = await _solicitudService.ListarSolicitudesAsync(tenantId, usuarioId, rol, filtros);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SolicitudDetalleDto>> ObtenerDetalle(Guid id)
    {
        var tenantId = ObtenerTenantId();
        var usuarioId = ObtenerUsuarioId();
        var rol = ObtenerRol();

        var detalle = await _solicitudService.ObtenerDetalleAsync(tenantId, usuarioId, rol, id);
        return Ok(detalle);
    }

    [HttpPost]
    public async Task<ActionResult<SolicitudDetalleDto>> Crear([FromBody] CrearSolicitudDto dto)
    {
        var tenantId = ObtenerTenantId();
        var usuarioId = ObtenerUsuarioId();

        var resultado = await _solicitudService.CrearSolicitudAsync(tenantId, usuarioId, dto);
        return CreatedAtAction(nameof(ObtenerDetalle), new { id = resultado.Id }, resultado);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SolicitudDetalleDto>> Editar(Guid id, [FromBody] EditarSolicitudDto dto)
    {
        var tenantId = ObtenerTenantId();
        var usuarioId = ObtenerUsuarioId();
        var rol = ObtenerRol();

        var resultado = await _solicitudService.EditarSolicitudAsync(tenantId, usuarioId, rol, id, dto);
        return Ok(resultado);
    }

    [HttpPost("{id:guid}/transiciones")]
    public async Task<ActionResult<SolicitudDetalleDto>> EjecutarTransicion(Guid id, [FromBody] EjecutarTransicionDto dto)
    {
        var tenantId = ObtenerTenantId();
        var usuarioId = ObtenerUsuarioId();
        var rol = ObtenerRol();

        var resultado = await _solicitudService.EjecutarTransicionAsync(tenantId, usuarioId, rol, id, dto);
        return Ok(resultado);
    }
}
