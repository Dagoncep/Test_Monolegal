using Monolegal.Api.Models;

namespace Monolegal.Api.Services;

public interface IFacturaService
{
    Task<List<FacturaResumen>> ObtenerResumenAsync();
}
