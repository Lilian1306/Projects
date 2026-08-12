using MesaSitec.Aplicacion.DTOs.Auth;

namespace MesaSitec.Aplicacion.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<UsuarioDto> GetPerfilAsync(Guid usuarioId);
}
