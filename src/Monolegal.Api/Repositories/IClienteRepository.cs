using Monolegal.Api.Models;

namespace Monolegal.Api.Repositories;

public interface IClienteRepository
{
    Task<List<Cliente>> ObtenerTodosAsync();
    Task<Cliente?> ObtenerPorIdAsync(string id);
}
