using MesaSitec.Dominio.Entidades;
using MesaSitec.Dominio.Enums;
using MesaSitec.Dominio.Excepciones;
using Xunit;

namespace MesaSitec.Tests.Dominio;

public class SolicitudTests
{
    // --- RN-02: Máquina de Estados ---

    [Fact]
    public void Transicion_DeNuevaAAsignada_CambiaEstadoExitosamente()
    {
        // Arrange
        var solicitud = new Solicitud { Estado = Estado.Nueva };
        var agenteId = Guid.NewGuid();

        // Act
        solicitud.Asignar(agenteId);

        // Assert
        Assert.Equal(Estado.Asignada, solicitud.Estado);
        Assert.Equal(agenteId, solicitud.AgenteId);
    }

    [Fact]
    public void Transicion_DeAsignadaAEnProceso_CambiaEstadoExitosamente()
    {
        // Arrange
        var solicitud = new Solicitud { Estado = Estado.Asignada };

        // Act
        solicitud.Iniciar();

        // Assert
        Assert.Equal(Estado.EnProceso, solicitud.Estado);
    }

    [Fact]
    public void Transicion_DeEnProcesoAResuelta_ConMotivoValido_CambiaEstadoExitosamente()
    {
        // Arrange
        var solicitud = new Solicitud { Estado = Estado.EnProceso };
        var motivoValido = "Se solucionó el inconveniente restableciendo la sesión del usuario.";
        var fechaResolucion = DateTime.UtcNow;

        // Act
        solicitud.Resolver(motivoValido, fechaResolucion);

        // Assert
        Assert.Equal(Estado.Resuelta, solicitud.Estado);
        Assert.Equal(motivoValido, solicitud.MotivoResolucion);
        Assert.Equal(fechaResolucion, solicitud.FechaResolucion);
    }

    [Fact]
    public void Transicion_Invalida_DeNuevaAResuelta_LanzaReglaNegocioException()
    {
        // Arrange
        var solicitud = new Solicitud { Estado = Estado.Nueva };

        // Act & Assert
        var ex = Assert.Throws<ReglaNegocioException>(() => 
            solicitud.Resolver("Motivo con más de veinte caracteres de prueba.", DateTime.UtcNow));

        Assert.Equal("TRANSICION_INVALIDA", ex.Codigo);
        Assert.Equal(409, ex.StatusCode);
    }

    // --- RN-04: Cálculo de SLA ---

    [Fact]
    public void CalcularSLA_IncidenteCritico_CalculaLimiteEnCuatroHoras()
    {
        // Arrange (Incidente = 8 horas base, Crítica = factor 0.5 -> 4 horas totales)
        var fechaCreacion = new DateTime(2026, 1, 15, 8, 0, 0, DateTimeKind.Utc);
        var solicitud = new Solicitud
        {
            FechaCreacion = fechaCreacion,
            Prioridad = Prioridad.Critica
        };

        // Act
        solicitud.CalcularFechaLimiteSla(slaHorasBase: 8);

        // Assert
        Assert.Equal(fechaCreacion.AddHours(4), solicitud.FechaLimiteSla);
    }

    [Fact]
    public void CalcularSLA_ConsultaBaja_CalculaLimiteEnCuarentaYOchoHoras()
    {
        // Arrange (Consulta = 24 horas base, Baja = factor 2.0 -> 48 horas totales)
        var fechaCreacion = new DateTime(2026, 1, 15, 8, 0, 0, DateTimeKind.Utc);
        var solicitud = new Solicitud
        {
            FechaCreacion = fechaCreacion,
            Prioridad = Prioridad.Baja
        };

        // Act
        solicitud.CalcularFechaLimiteSla(slaHorasBase: 24);

        // Assert
        Assert.Equal(fechaCreacion.AddHours(48), solicitud.FechaLimiteSla);
    }

    [Fact]
    public void EsVencida_RetornaTrue_CuandoSlaExpiroYEstadoEstaActivo()
    {
        // Arrange
        var fechaCreacion = new DateTime(2026, 1, 15, 8, 0, 0, DateTimeKind.Utc);
        var solicitud = new Solicitud
        {
            FechaCreacion = fechaCreacion,
            Prioridad = Prioridad.Media,
            Estado = Estado.Nueva
        };
        solicitud.CalcularFechaLimiteSla(slaHorasBase: 8); // Límite: 2026-01-15 16:00 UTC

        var fechaActual = fechaCreacion.AddHours(10); // 2026-01-15 18:00 UTC (Vencida)

        // Act & Assert
        Assert.True(solicitud.EsVencida(fechaActual));
    }

    [Fact]
    public void EsVencida_RetornaFalse_CuandoSolicitudEstaResuelta()
    {
        // Arrange
        var fechaCreacion = new DateTime(2026, 1, 15, 8, 0, 0, DateTimeKind.Utc);
        var solicitud = new Solicitud
        {
            FechaCreacion = fechaCreacion,
            Prioridad = Prioridad.Media,
            Estado = Estado.Resuelta
        };
        solicitud.CalcularFechaLimiteSla(slaHorasBase: 8);

        var fechaActual = fechaCreacion.AddHours(10); // Pasó la fecha límite, pero ya está Resuelta

        // Act & Assert
        Assert.False(solicitud.EsVencida(fechaActual));
    }

    // --- RN-06: Cierre con Justificación ---

    [Fact]
    public void Resolver_SinMotivoSuficiente_LanzaExcepcionMotivoRequerido()
    {
        // Arrange
        var solicitud = new Solicitud { Estado = Estado.EnProceso };
        var motivoCorto = "Corto"; // Menos de 20 caracteres

        // Act & Assert
        var ex = Assert.Throws<ReglaNegocioException>(() => 
            solicitud.Resolver(motivoCorto, DateTime.UtcNow));

        Assert.Equal("MOTIVO_REQUERIDO", ex.Codigo);
        Assert.Equal(422, ex.StatusCode);
    }

    [Fact]
    public void Cancelar_SinMotivoSuficiente_LanzaExcepcionMotivoRequerido()
    {
        // Arrange
        var solicitud = new Solicitud { Estado = Estado.Nueva };
        var motivoCorto = "Error"; // Menos de 10 caracteres

        // Act & Assert
        var ex = Assert.Throws<ReglaNegocioException>(() => 
            solicitud.Cancelar(motivoCorto));

        Assert.Equal("MOTIVO_REQUERIDO", ex.Codigo);
        Assert.Equal(422, ex.StatusCode);
    }
}
