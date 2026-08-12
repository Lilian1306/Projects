namespace MesaSitec.Aplicacion.DTOs.Solicitudes;

public class EjecutarTransicionDto
{
    public string Accion { get; set; } = string.Empty; 
    public Guid? AgenteId { get; set; }
    public string? Motivo { get; set; }
}
