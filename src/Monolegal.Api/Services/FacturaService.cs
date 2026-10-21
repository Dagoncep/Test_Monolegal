using Monolegal.Api.Models;
using Monolegal.Api.Repositories;

namespace Monolegal.Api.Services;

// Junta cada factura con los datos de su cliente para mostrarlas en el frontend
public class FacturaService : IFacturaService
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IClienteRepository _clienteRepository;

    public FacturaService(IFacturaRepository facturaRepository, IClienteRepository clienteRepository)
    {
        _facturaRepository = facturaRepository;
        _clienteRepository = clienteRepository;
    }

    public async Task<List<FacturaResumen>> ObtenerResumenAsync()
    {
        var facturas = await _facturaRepository.ObtenerTodasAsync();
        var clientes = await _clienteRepository.ObtenerTodosAsync();

        return facturas
            .Select(factura =>
            {
                var cliente = clientes.FirstOrDefault(c => c.Id == factura.ClienteId);
                return new FacturaResumen
                {
                    Numero = factura.Numero,
                    ClienteId = factura.ClienteId,
                    ClienteNombre = cliente?.Nombre ?? "Cliente no encontrado",
                    ClienteEmail = cliente?.Email,
                    Monto = factura.Monto,
                    Estado = factura.Estado,
                    FechaEmision = factura.FechaEmision,
                    FechaVencimiento = factura.FechaVencimiento,
                    FechaActualizacion = factura.FechaActualizacion
                };
            })
            .OrderBy(f => f.ClienteNombre)
            .ThenBy(f => f.Numero)
            .ToList();
    }
}
