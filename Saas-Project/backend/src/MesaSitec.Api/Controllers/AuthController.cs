using MesaSitec.Aplicacion.DTOs.Auth;
using MesaSitec.Aplicacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MesaSitec.Api.Controllers;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpGet("perfil")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> GetPerfil()
    {
        var usuarioId = ObtenerUsuarioId();
        var perfil = await _authService.GetPerfilAsync(usuarioId);
        return Ok(perfil);
    }

    [HttpGet("/api/v1/me")]
    [Authorize]
    public async Task<ActionResult<UsuarioDto>> GetMe()
    {
        return await GetPerfil();
    }
}
