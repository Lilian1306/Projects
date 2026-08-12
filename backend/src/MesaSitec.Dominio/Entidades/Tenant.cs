namespace MesaSitec.Dominio.Entidades;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
