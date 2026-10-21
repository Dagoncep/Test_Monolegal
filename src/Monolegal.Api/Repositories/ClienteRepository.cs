using MongoDB.Driver;
using Monolegal.Api.Models;

namespace Monolegal.Api.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly IMongoCollection<Cliente> _clientes;

    public ClienteRepository(IMongoDatabase database)
    {
        _clientes = database.GetCollection<Cliente>("clientes");
    }

    public Task<List<Cliente>> ObtenerTodosAsync()
    {
        return _clientes.Find(_ => true).ToListAsync();
    }

    public async Task<Cliente?> ObtenerPorIdAsync(string id)
    {
        return await _clientes.Find(c => c.Id == id).FirstOrDefaultAsync();
    }
}
