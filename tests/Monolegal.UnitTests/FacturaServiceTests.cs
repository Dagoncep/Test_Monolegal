using Monolegal.Api.Models;
using Monolegal.Api.Repositories;
using Monolegal.Api.Services;
using Moq;

namespace Monolegal.UnitTests;

public class FacturaServiceTests
{
    private readonly Mock<IFacturaRepository> _facturaRepository = new();
    private readonly Mock<IClienteRepository> _clienteRepository = new();

    [Fact]
    public async Task ObtenerResumen_JuntaCadaFacturaConSuCliente()
    {
        var cliente = DatosPrueba.Cliente("1020304050", "Laura Gómez", "laura@correo.test");
        ConfigurarDatos([cliente], [DatosPrueba.Factura("FAC-2001", cliente.Id, EstadosFactura.Pagada, 250000m)]);

        var resumen = Assert.Single(await CrearServicio().ObtenerResumenAsync());

        Assert.Equal("FAC-2001", resumen.Numero);
        Assert.Equal("Laura Gómez", resumen.ClienteNombre);
        Assert.Equal("laura@correo.test", resumen.ClienteEmail);
        Assert.Equal(250000m, resumen.Monto);
        Assert.Equal(EstadosFactura.Pagada, resumen.Estado);
    }

    [Fact]
    public async Task ObtenerResumen_FacturaConClienteInexistente_SeMuestraIgual()
    {
        ConfigurarDatos([], [DatosPrueba.Factura(clienteId: "999")]);

        var resumen = Assert.Single(await CrearServicio().ObtenerResumenAsync());

        Assert.Equal("Cliente no encontrado", resumen.ClienteNombre);
    }

    [Fact]
    public async Task ObtenerResumen_OrdenaPorClienteYNumero()
    {
        var zeta = DatosPrueba.Cliente("2", "Zeta");
        var alfa = DatosPrueba.Cliente("1", "Alfa");
        ConfigurarDatos(
            [zeta, alfa],
            [DatosPrueba.Factura("FAC-3", zeta.Id), DatosPrueba.Factura("FAC-2", alfa.Id), DatosPrueba.Factura("FAC-1", alfa.Id)]);

        var resumen = await CrearServicio().ObtenerResumenAsync();

        Assert.Equal(["FAC-1", "FAC-2", "FAC-3"], resumen.Select(r => r.Numero));
    }

    private void ConfigurarDatos(List<Cliente> clientes, List<Factura> facturas)
    {
        _facturaRepository.Setup(r => r.ObtenerTodasAsync()).ReturnsAsync(facturas);
        _clienteRepository.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(clientes);
    }

    private FacturaService CrearServicio() => new(_facturaRepository.Object, _clienteRepository.Object);
}
