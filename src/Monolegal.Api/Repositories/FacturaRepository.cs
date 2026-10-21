using MongoDB.Driver;
using Monolegal.Api.Models;

namespace Monolegal.Api.Repositories;

public class FacturaRepository : IFacturaRepository
{
    private readonly IMongoCollection<Factura> _facturas;

    public FacturaRepository(IMongoDatabase database)
    {
        _facturas = database.GetCollection<Factura>("facturas");
    }

    public Task<List<Factura>> ObtenerTodasAsync()
    {
        return _facturas.Find(_ => true).ToListAsync();
    }

    public Task<List<Factura>> ObtenerPorEstadosAsync(params string[] estados)
    {
        return _facturas.Find(f => estados.Contains(f.Estado)).ToListAsync();
    }

    public async Task<bool> ActualizarEstadoAsync(string numero, string estadoActual, string nuevoEstado)
    {
        // Filtrar también por el estado actual evita aplicar dos veces el mismo cambio
        var filtro = Builders<Factura>.Filter.Where(f => f.Numero == numero && f.Estado == estadoActual);

        var cambios = Builders<Factura>.Update
            .Set(f => f.Estado, nuevoEstado)
            .Set(f => f.FechaActualizacion, DateTime.UtcNow);

        var resultado = await _facturas.UpdateOneAsync(filtro, cambios);
        return resultado.ModifiedCount == 1;
    }
}
