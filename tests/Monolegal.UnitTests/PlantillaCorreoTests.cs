using Monolegal.Api.Models;
using Monolegal.Api.Services;

namespace Monolegal.UnitTests;

public class PlantillaCorreoTests
{
    [Fact]
    public void PrimerRecordatorio_AvisaQuePasaASegundoRecordatorio()
    {
        var factura = DatosPrueba.Factura("FAC-77", estado: EstadosFactura.PrimerRecordatorio);

        var asunto = PlantillaCorreo.CrearAsunto(factura);
        var cuerpo = PlantillaCorreo.CrearCuerpo(DatosPrueba.Cliente(), factura);

        Assert.Equal("Segundo recordatorio de pago - Factura FAC-77", asunto);
        Assert.Contains("ha pasado a segundo recordatorio", cuerpo);
    }

    [Fact]
    public void SegundoRecordatorio_AvisaQueElServicioSeraDesactivado()
    {
        var factura = DatosPrueba.Factura("FAC-88", estado: EstadosFactura.SegundoRecordatorio);

        var asunto = PlantillaCorreo.CrearAsunto(factura);
        var cuerpo = PlantillaCorreo.CrearCuerpo(DatosPrueba.Cliente(), factura);

        Assert.Equal("Aviso de desactivación del servicio - Factura FAC-88", asunto);
        Assert.Contains("va a ser desactivado", cuerpo);
    }

    [Fact]
    public void Cuerpo_IncluyeElNombreDelClienteYLosDatosDeLaFactura()
    {
        var cliente = DatosPrueba.Cliente(nombre: "Laura Restrepo");
        var factura = DatosPrueba.Factura("FAC-2001");

        var cuerpo = PlantillaCorreo.CrearCuerpo(cliente, factura);

        Assert.Contains("Hola Laura Restrepo", cuerpo);
        Assert.Contains("FAC-2001", cuerpo);
        Assert.Contains("30/09/2026", cuerpo);
    }

    [Fact]
    public void Cuerpo_CodificaElNombreParaQueNoRompaElHtml()
    {
        var cliente = DatosPrueba.Cliente(nombre: "<script>alert(1)</script>");

        var cuerpo = PlantillaCorreo.CrearCuerpo(cliente, DatosPrueba.Factura());

        Assert.DoesNotContain("<script>", cuerpo);
    }

    [Theory]
    [InlineData(EstadosFactura.Pagada)]
    [InlineData(EstadosFactura.Desactivado)]
    public void EstadoSinRecordatorio_LanzaExcepcion(string estado)
    {
        var factura = DatosPrueba.Factura(estado: estado);

        Assert.Throws<ArgumentException>(() => PlantillaCorreo.CrearAsunto(factura));
    }
}
