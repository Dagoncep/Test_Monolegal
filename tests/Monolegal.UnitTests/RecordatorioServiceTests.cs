using Microsoft.Extensions.Logging.Abstractions;
using Monolegal.Api.Models;
using Monolegal.Api.Repositories;
using Monolegal.Api.Services;
using Moq;

namespace Monolegal.UnitTests;

public class RecordatorioServiceTests
{
    // Mocks: objetos falsos que reemplazan a MongoDB y al servidor de correo
    private readonly Mock<IFacturaRepository> _facturaRepository = new();
    private readonly Mock<IClienteRepository> _clienteRepository = new();
    private readonly Mock<IEmailService> _emailService = new();

    private readonly RecordatorioService _servicio;

    public RecordatorioServiceTests()
    {
        // Por defecto, cualquier actualización de estado funciona
        _facturaRepository
            .Setup(r => r.ActualizarEstadoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _servicio = new RecordatorioService(
            _facturaRepository.Object,
            _clienteRepository.Object,
            _emailService.Object,
            NullLogger<RecordatorioService>.Instance);
    }

    [Fact]
    public async Task PrimerRecordatorio_EnviaCorreoYPasaASegundoRecordatorio()
    {
        var cliente = DatosPrueba.Cliente(email: "cliente@correo.test");
        var factura = DatosPrueba.Factura("FAC-1", cliente.Id, EstadosFactura.PrimerRecordatorio);
        ConfigurarDatos([cliente], [factura]);

        var resultado = await _servicio.ProcesarAsync();

        _emailService.Verify(e => e.EnviarAsync("cliente@correo.test", It.Is<string>(a => a.Contains("Segundo recordatorio")), It.IsAny<string>()), Times.Once);
        _facturaRepository.Verify(r => r.ActualizarEstadoAsync("FAC-1", EstadosFactura.PrimerRecordatorio, EstadosFactura.SegundoRecordatorio), Times.Once);
        Assert.Equal(1, resultado.TotalExitosas);
    }

    [Fact]
    public async Task SegundoRecordatorio_EnviaCorreoYPasaADesactivado()
    {
        var cliente = DatosPrueba.Cliente();
        var factura = DatosPrueba.Factura("FAC-1", cliente.Id, EstadosFactura.SegundoRecordatorio);
        ConfigurarDatos([cliente], [factura]);

        var resultado = await _servicio.ProcesarAsync();

        _emailService.Verify(e => e.EnviarAsync(cliente.Email, It.Is<string>(a => a.Contains("desactivación")), It.IsAny<string>()), Times.Once);
        _facturaRepository.Verify(r => r.ActualizarEstadoAsync("FAC-1", EstadosFactura.SegundoRecordatorio, EstadosFactura.Desactivado), Times.Once);
        var detalle = Assert.Single(resultado.Detalle);
        Assert.Equal(EstadosFactura.Desactivado, detalle.EstadoNuevo);
    }

    [Fact]
    public async Task SoloBuscaFacturasEnPrimerOSegundoRecordatorio()
    {
        ConfigurarDatos([], []);

        await _servicio.ProcesarAsync();

        _facturaRepository.Verify(r => r.ObtenerPorEstadosAsync(EstadosFactura.PrimerRecordatorio, EstadosFactura.SegundoRecordatorio), Times.Once);
    }

    [Fact]
    public async Task TresClientes_CadaUnoRecibeElCorreoDeSuFactura()
    {
        var andinas = DatosPrueba.Cliente("900123456", "Inversiones Andinas", "cartera@andinas.test");
        var laura = DatosPrueba.Cliente("1020304050", "Laura Gómez", "laura@correo.test");
        var caribe = DatosPrueba.Cliente("800987654", "Tecnologías del Caribe", "pagos@tecaribe.test");
        ConfigurarDatos(
            [andinas, laura, caribe],
            [
                DatosPrueba.Factura("FAC-1001", andinas.Id, EstadosFactura.PrimerRecordatorio),
                DatosPrueba.Factura("FAC-2001", laura.Id, EstadosFactura.SegundoRecordatorio),
                DatosPrueba.Factura("FAC-3001", caribe.Id, EstadosFactura.PrimerRecordatorio)
            ]);

        var resultado = await _servicio.ProcesarAsync();

        Assert.Equal(3, resultado.TotalExitosas);
        _emailService.Verify(e => e.EnviarAsync("cartera@andinas.test", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _emailService.Verify(e => e.EnviarAsync("laura@correo.test", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _emailService.Verify(e => e.EnviarAsync("pagos@tecaribe.test", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SiFallaElCorreo_NoActualizaElEstadoYSigueConLasDemas()
    {
        var clienteConError = DatosPrueba.Cliente("111", email: "error@correo.test");
        var clienteOk = DatosPrueba.Cliente("222", email: "ok@correo.test");
        ConfigurarDatos(
            [clienteConError, clienteOk],
            [DatosPrueba.Factura("FAC-1", "111"), DatosPrueba.Factura("FAC-2", "222")]);
        _emailService
            .Setup(e => e.EnviarAsync("error@correo.test", It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Servidor de correo no disponible"));

        var resultado = await _servicio.ProcesarAsync();

        _facturaRepository.Verify(r => r.ActualizarEstadoAsync("FAC-1", It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _facturaRepository.Verify(r => r.ActualizarEstadoAsync("FAC-2", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        Assert.Equal(1, resultado.TotalExitosas);
        Assert.Equal(1, resultado.TotalFallidas);
    }

    [Fact]
    public async Task ClienteInexistente_NoEnviaCorreoNiActualiza()
    {
        ConfigurarDatos([], [DatosPrueba.Factura(clienteId: "999")]);

        var resultado = await _servicio.ProcesarAsync();

        _emailService.Verify(e => e.EnviarAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _facturaRepository.Verify(r => r.ActualizarEstadoAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        Assert.Equal(1, resultado.TotalFallidas);
    }

    [Fact]
    public async Task SiLaFacturaYaCambioDeEstado_ReportaError()
    {
        var cliente = DatosPrueba.Cliente();
        ConfigurarDatos([cliente], [DatosPrueba.Factura("FAC-1", cliente.Id)]);
        _facturaRepository
            .Setup(r => r.ActualizarEstadoAsync("FAC-1", It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var resultado = await _servicio.ProcesarAsync();

        var detalle = Assert.Single(resultado.Detalle);
        Assert.False(detalle.Exitoso);
        Assert.Contains("ya había cambiado", detalle.Error);
    }

    [Fact]
    public async Task SinFacturasPendientes_NoEnviaCorreos()
    {
        ConfigurarDatos([], []);

        var resultado = await _servicio.ProcesarAsync();

        Assert.Equal(0, resultado.TotalProcesadas);
        _emailService.Verify(e => e.EnviarAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    // Le dice a los mocks qué facturas y clientes "hay en la base de datos"
    private void ConfigurarDatos(List<Cliente> clientes, List<Factura> facturas)
    {
        _facturaRepository
            .Setup(r => r.ObtenerPorEstadosAsync(It.IsAny<string[]>()))
            .ReturnsAsync(facturas);

        _clienteRepository
            .Setup(r => r.ObtenerPorIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => clientes.FirstOrDefault(c => c.Id == id));
    }
}
