using MesaSitec.Aplicacion.DTOs.Comun;

namespace MesaSitec.Aplicacion.DTOs.Solicitudes;

public class SolicitudDetalleDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public ResumenRefDto Categoria { get; set; } = null!;
    public ResumenRefDto Solicitante { get; set; } = null!;
    public ResumenRefDto? Agente { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaLimiteSla { get; set; }
    public bool Vencida { get; set; }
    public string? MotivoResolucion { get; set; }
    public string? MotivoCancelacion { get; set; }
    public DateTime? FechaResolucion { get; set; }
}
