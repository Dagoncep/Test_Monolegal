using Microsoft.AspNetCore.Mvc;
using Monolegal.Api.Controllers;
using Monolegal.Api.Models;
using Monolegal.Api.Services;
using Moq;

namespace Monolegal.UnitTests;

public class ControllersTests
{
    [Fact]
    public async Task FacturasController_DevuelveOkConElResumen()
    {
        var facturas = new List<FacturaResumen> { new() { Numero = "FAC-1" } };
        var servicio = new Mock<IFacturaService>();
        servicio.Setup(s => s.ObtenerResumenAsync()).ReturnsAsync(facturas);

        var respuesta = await new FacturasController(servicio.Object).ObtenerResumen();

        var ok = Assert.IsType<OkObjectResult>(respuesta.Result);
        Assert.Same(facturas, ok.Value);
    }

    [Fact]
    public async Task RecordatoriosController_EjecutaElProcesoYDevuelveElResultado()
    {
        var resultado = new ResultadoProcesamiento();
        var servicio = new Mock<IRecordatorioService>();
        servicio.Setup(s => s.ProcesarAsync()).ReturnsAsync(resultado);

        var respuesta = await new RecordatoriosController(servicio.Object).Procesar();

        var ok = Assert.IsType<OkObjectResult>(respuesta.Result);
        Assert.Same(resultado, ok.Value);
        servicio.Verify(s => s.ProcesarAsync(), Times.Once);
    }
}
