using MesaSitec.Dominio.Entidades;

namespace MesaSitec.Aplicacion.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerarToken(Usuario usuario);
}
