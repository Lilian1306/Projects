using MesaSitec.Aplicacion.DTOs.Auth;
using MesaSitec.Aplicacion.Interfaces;
using MesaSitec.Dominio.Excepciones;
using Microsoft.EntityFrameworkCore;

namespace MesaSitec.Aplicacion.Servicios;

public class AuthService : IAuthService
{
    private readonly DbContext _context;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(DbContext context, IJwtTokenGenerator jwtTokenGenerator)
    {
        _context = context;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ReglaNegocioException("NO_AUTENTICADO", "Credenciales incorrectas.", 401);
        }

        var usuario = await _context.Set<MesaSitec.Dominio.Entidades.Usuario>()
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.Trim().ToLower());

        if (usuario == null || !usuario.Activo)
        {
            throw new ReglaNegocioException("NO_AUTENTICADO", "Credenciales incorrectas.", 401);
        }

        bool passwordValido = BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash);
        if (!passwordValido)
        {
            throw new ReglaNegocioException("NO_AUTENTICADO", "Credenciales incorrectas.", 401);
        }

        string token = _jwtTokenGenerator.GenerarToken(usuario);

        return new LoginResponseDto
        {
            AccessToken = token,
            ExpiraEn = 28800,
            Usuario = new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol.ToString(),
                TenantId = usuario.TenantId,
                TenantNombre = usuario.Tenant?.Nombre ?? string.Empty
            }
        };
    }

    public async Task<UsuarioDto> GetPerfilAsync(Guid usuarioId)
    {
        var usuario = await _context.Set<MesaSitec.Dominio.Entidades.Usuario>()
            .Include(u => u.Tenant)
            .FirstOrDefaultAsync(u => u.Id == usuarioId);

        if (usuario == null || !usuario.Activo)
        {
            throw new ReglaNegocioException("NO_AUTENTICADO", "Usuario no encontrado o inactivo.", 401);
        }

        return new UsuarioDto
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol.ToString(),
            TenantId = usuario.TenantId,
            TenantNombre = usuario.Tenant?.Nombre ?? string.Empty
        };
    }
}
