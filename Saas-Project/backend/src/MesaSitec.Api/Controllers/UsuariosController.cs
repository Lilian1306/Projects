using MesaSitec.Aplicacion.DTOs.Usuarios;
using MesaSitec.Aplicacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesaSitec.Api.Controllers;

[Authorize]
public class UsuariosController : BaseApiController
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet("agentes")]
    public async Task<ActionResult<IEnumerable<AgenteDto>>> ObtenerAgentes()
    {
        var tenantId = ObtenerTenantId();
        var agentes = await _usuarioService.ObtenerAgentesTenantAsync(tenantId);
        return Ok(agentes);
    }
}
