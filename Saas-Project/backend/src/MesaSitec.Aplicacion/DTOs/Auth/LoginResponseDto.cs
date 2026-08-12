namespace MesaSitec.Aplicacion.DTOs.Auth;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public int ExpiraEn { get; set; } = 28800; 
    public UsuarioDto Usuario { get; set; } = null!;
}
