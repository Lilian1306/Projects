using MesaSitec.Aplicacion.Servicios;
using MesaSitec.Dominio.Enums;
using MesaSitec.Dominio.Excepciones;
using Xunit;

namespace MesaSitec.Tests.Aplicacion;

public class PermisoTransicionTests
{
    private static readonly Guid SolicitanteId = Guid.NewGuid();
    private static readonly Guid OtroUsuarioId = Guid.NewGuid();

    [Theory]
    [InlineData(Rol.Admin, "asignar")]
    [InlineData(Rol.Agente, "asignar")]
    [InlineData(Rol.Admin, "iniciar")]
    [InlineData(Rol.Agente, "iniciar")]
    [InlineData(Rol.Admin, "resolver")]
    [InlineData(Rol.Agente, "resolver")]
    [InlineData(Rol.Admin, "reabrir")]
    [InlineData(Rol.Agente, "reabrir")]
    public void AccionesDeGestion_PermitidasParaAdminYAgente(Rol rol, string accion)
    {
        var ex = Record.Exception(() =>
            SolicitudService.ValidarPermisoTransicion(rol, SolicitanteId, OtroUsuarioId, accion));

        Assert.Null(ex);
    }

    [Theory]
    [InlineData("asignar")]
    [InlineData("iniciar")]
    [InlineData("resolver")]
    [InlineData("reabrir")]
    public void AccionesDeGestion_ParaSolicitante_LanzaOperacionNoPermitida(string accion)
    {
        var ex = Assert.Throws<ReglaNegocioException>(() =>
            SolicitudService.ValidarPermisoTransicion(Rol.Solicitante, SolicitanteId, SolicitanteId, accion));

        Assert.Equal("OPERACION_NO_PERMITIDA", ex.Codigo);
        Assert.Equal(403, ex.StatusCode);
    }

    [Fact]
    public void Cancelar_PermitidoSoloParaAdmin()
    {
        var ex = Record.Exception(() =>
            SolicitudService.ValidarPermisoTransicion(Rol.Admin, SolicitanteId, OtroUsuarioId, "cancelar"));

        Assert.Null(ex);
    }

    [Fact]
    public void Cancelar_NoPermitidoParaAgente_LanzaOperacionNoPermitida()
    {
        var ex = Assert.Throws<ReglaNegocioException>(() =>
            SolicitudService.ValidarPermisoTransicion(Rol.Agente, SolicitanteId, OtroUsuarioId, "cancelar"));

        Assert.Equal("OPERACION_NO_PERMITIDA", ex.Codigo);
    }

    [Fact]
    public void Cancelar_NoPermitidoParaSolicitante_LanzaOperacionNoPermitida()
    {
        var ex = Assert.Throws<ReglaNegocioException>(() =>
            SolicitudService.ValidarPermisoTransicion(Rol.Solicitante, SolicitanteId, SolicitanteId, "cancelar"));

        Assert.Equal("OPERACION_NO_PERMITIDA", ex.Codigo);
    }

    [Fact]
    public void Cerrar_PermitidoParaSolicitantePropietario()
    {
        var ex = Record.Exception(() =>
            SolicitudService.ValidarPermisoTransicion(Rol.Solicitante, SolicitanteId, SolicitanteId, "cerrar"));

        Assert.Null(ex);
    }

    [Fact]
    public void Cerrar_NoPermitidoParaSolicitanteQueNoEsElPropietario()
    {
        var ex = Assert.Throws<ReglaNegocioException>(() =>
            SolicitudService.ValidarPermisoTransicion(Rol.Solicitante, SolicitanteId, OtroUsuarioId, "cerrar"));

        Assert.Equal("OPERACION_NO_PERMITIDA", ex.Codigo);
    }

    [Theory]
    [InlineData(Rol.Admin)]
    [InlineData(Rol.Agente)]
    public void Cerrar_PermitidoParaAdminYAgente_SinImportarPropietario(Rol rol)
    {
        var ex = Record.Exception(() =>
            SolicitudService.ValidarPermisoTransicion(rol, SolicitanteId, OtroUsuarioId, "cerrar"));

        Assert.Null(ex);
    }
}
