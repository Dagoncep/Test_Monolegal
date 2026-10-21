using Monolegal.Api.Models;

namespace Monolegal.Api.Repositories;

public interface IFacturaRepository
{
    Task<List<Factura>> ObtenerTodasAsync();
    Task<List<Factura>> ObtenerPorEstadosAsync(params string[] estados);

    // Cambia el estado solo si la factura sigue en estadoActual.
    // Devuelve false si no se actualizó (por ejemplo, otro proceso ya la cambió).
    Task<bool> ActualizarEstadoAsync(string numero, string estadoActual, string nuevoEstado);
}
