using MesaSitec.Dominio.Enums;
using MesaSitec.Dominio.Excepciones;

namespace MesaSitec.Dominio.Entidades;

public class Solicitud
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public Guid CategoriaId { get; set; }
    public Prioridad Prioridad { get; set; } = Prioridad.Media;
    public Estado Estado { get; set; } = Estado.Nueva;
    
    public Guid SolicitanteId { get; set; }
    public Guid? AgenteId { get; set; }
    
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime FechaLimiteSla { get; set; }
    public DateTime? FechaResolucion { get; set; }
    
    public string? MotivoResolucion { get; set; }
    public string? MotivoCancelacion { get; set; }

    public Categoria? Categoria { get; set; }
    public Usuario? Solicitante { get; set; }
    public Usuario? Agente { get; set; }
    public Tenant? Tenant { get; set; }

    public void CalcularFechaLimiteSla(int slaHorasBase)
    {
        double factor = Prioridad switch
        {
            Prioridad.Critica => 0.5,
            Prioridad.Alta => 0.75,
            Prioridad.Media => 1.0,
            Prioridad.Baja => 2.0,
            _ => 1.0
        };

        double horasTotales = slaHorasBase * factor;
        FechaLimiteSla = FechaCreacion.AddHours(horasTotales);
    }

    public bool EsVencida(DateTime? fechaComparacion = null)
    {
        var referencia = fechaComparacion ?? DateTime.UtcNow;

        if (Estado == Estado.Resuelta || Estado == Estado.Cerrada || Estado == Estado.Cancelada)
        {
            return false;
        }

        return referencia > FechaLimiteSla;
    }

    public void Asignar(Guid nuevoAgenteId)
    {
        if (Estado != Estado.Nueva && Estado != Estado.Asignada && Estado != Estado.EnProceso)
        {
            throw new ReglaNegocioException(
                "TRANSICION_INVALIDA",
                $"No se puede aplicar 'asignar' sobre una solicitud en estado '{Estado}'.",
                statusCode: 409);
        }

        AgenteId = nuevoAgenteId;
        Estado = Estado.Asignada;
    }

    public void Iniciar()
    {
        if (Estado != Estado.Asignada)
        {
            throw new ReglaNegocioException(
                "TRANSICION_INVALIDA",
                $"No se puede aplicar 'iniciar' sobre una solicitud en estado '{Estado}'. Debe estar en 'Asignada'.",
                statusCode: 409);
        }

        Estado = Estado.EnProceso;
    }

    public void Resolver(string motivo, DateTime fechaResolucion)
    {
        if (Estado != Estado.EnProceso)
        {
            throw new ReglaNegocioException(
                "TRANSICION_INVALIDA",
                $"No se puede aplicar 'resolver' sobre una solicitud en estado '{Estado}'. Debe estar en 'EnProceso'.",
                statusCode: 409);
        }

        if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length < 20)
        {
            throw new ReglaNegocioException(
                "MOTIVO_REQUERIDO",
                "El motivo de resolución debe tener al menos 20 caracteres.",
                statusCode: 422);
        }

        MotivoResolucion = motivo.Trim();
        FechaResolucion = fechaResolucion;
        Estado = Estado.Resuelta;
    }

    public void Cerrar()
    {
        if (Estado != Estado.Resuelta)
        {
            throw new ReglaNegocioException(
                "TRANSICION_INVALIDA",
                $"No se puede aplicar 'cerrar' sobre una solicitud en estado '{Estado}'. Debe estar en 'Resuelta'.",
                statusCode: 409);
        }

        Estado = Estado.Cerrada;
    }

    public void Reabrir()
    {
        if (Estado != Estado.Resuelta)
        {
            throw new ReglaNegocioException(
                "TRANSICION_INVALIDA",
                $"No se puede aplicar 'reabrir' sobre una solicitud en estado '{Estado}'. Debe estar en 'Resuelta'.",
                statusCode: 409);
        }

        Estado = Estado.EnProceso;
    }

    public void Cancelar(string motivo)
    {
        if (Estado != Estado.Nueva && Estado != Estado.Asignada && Estado != Estado.EnProceso)
        {
            throw new ReglaNegocioException(
                "TRANSICION_INVALIDA",
                $"No se puede aplicar 'cancelar' sobre una solicitud en estado '{Estado}'.",
                statusCode: 409);
        }

        if (string.IsNullOrWhiteSpace(motivo) || motivo.Trim().Length < 10)
        {
            throw new ReglaNegocioException(
                "MOTIVO_REQUERIDO",
                "El motivo de cancelación debe tener al menos 10 caracteres.",
                statusCode: 422);
        }

        MotivoCancelacion = motivo.Trim();
        Estado = Estado.Cancelada;
    }
}
