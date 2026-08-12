using System.Security.Claims;
using MesaSitec.Dominio.Enums;
using MesaSitec.Dominio.Excepciones;
using Microsoft.AspNetCore.Mvc;

namespace MesaSitec.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected Guid ObtenerUsuarioId()
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(subClaim) || !Guid.TryParse(subClaim, out var usuarioId))
        {
            throw new ReglaNegocioException("NO_AUTENTICADO", "Usuario no autenticado.", 401);
        }

        return usuarioId;
    }

    protected Guid ObtenerTenantId()
    {
        var tenantClaim = User.FindFirst("tenantId")?.Value;

        if (string.IsNullOrEmpty(tenantClaim) || !Guid.TryParse(tenantClaim, out var tenantId))
        {
            throw new ReglaNegocioException("NO_AUTENTICADO", "Tenant no válido en el token de autenticación.", 401);
        }

        return tenantId;
    }

    protected Rol ObtenerRol()
    {
        var rolClaim = User.FindFirst("rol")?.Value 
            ?? User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(rolClaim) || !Enum.TryParse<Rol>(rolClaim, ignoreCase: true, out var rol))
        {
            throw new ReglaNegocioException("NO_AUTENTICADO", "Rol no válido en el token de autenticación.", 401);
        }

        return rol;
    }
}
