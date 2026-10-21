using Monolegal.Api.Models;
using Monolegal.Api.Repositories;

namespace Monolegal.Api.Services;

// Proceso principal: por cada factura en primer o segundo recordatorio envía el correo
// y, solo si el envío funcionó, pasa la factura al siguiente estado.
public class RecordatorioService : IRecordatorioService
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<RecordatorioService> _logger;

    public RecordatorioService(
        IFacturaRepository facturaRepository,
        IClienteRepository clienteRepository,
        IEmailService emailService,
        ILogger<RecordatorioService> logger)
    {
        _facturaRepository = facturaRepository;
        _clienteRepository = clienteRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<ResultadoProcesamiento> ProcesarAsync()
    {
        var facturas = await _facturaRepository.ObtenerPorEstadosAsync(
            EstadosFactura.PrimerRecordatorio,
            EstadosFactura.SegundoRecordatorio);

        var resultado = new ResultadoProcesamiento();
        foreach (var factura in facturas)
        {
            resultado.Detalle.Add(await ProcesarFacturaAsync(factura));
        }

        return resultado;
    }

    private async Task<ResultadoFactura> ProcesarFacturaAsync(Factura factura)
    {
        var cliente = await _clienteRepository.ObtenerPorIdAsync(factura.ClienteId);
        if (cliente is null)
        {
            return Fallida(factura, "No se encontró el cliente de la factura.");
        }

        // 1. Enviar el correo. Si falla, la factura se deja como estaba.
        try
        {
            await _emailService.EnviarAsync(
                cliente.Email,
                PlantillaCorreo.CrearAsunto(factura),
                PlantillaCorreo.CrearCuerpo(cliente, factura));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo enviar el correo de la factura {Numero}", factura.Numero);
            return Fallida(factura, "No fue posible enviar el correo; el estado no se modificó.");
        }

        // 2. Actualizar el estado, ahora que el correo ya se envió
        var nuevoEstado = ObtenerSiguienteEstado(factura.Estado);
        var actualizada = await _facturaRepository.ActualizarEstadoAsync(factura.Numero, factura.Estado, nuevoEstado);
        if (!actualizada)
        {
            return Fallida(factura, "El correo se envió, pero la factura ya había cambiado de estado.");
        }

        return new ResultadoFactura
        {
            Numero = factura.Numero,
            EstadoAnterior = factura.Estado,
            EstadoNuevo = nuevoEstado,
            Exitoso = true
        };
    }

    private static string ObtenerSiguienteEstado(string estadoActual)
    {
        return estadoActual switch
        {
            EstadosFactura.PrimerRecordatorio => EstadosFactura.SegundoRecordatorio,
            EstadosFactura.SegundoRecordatorio => EstadosFactura.Desactivado,
            _ => throw new ArgumentException($"El estado '{estadoActual}' no tiene un siguiente estado.")
        };
    }

    private static ResultadoFactura Fallida(Factura factura, string error)
    {
        return new ResultadoFactura
        {
            Numero = factura.Numero,
            EstadoAnterior = factura.Estado,
            Exitoso = false,
            Error = error
        };
    }
}
