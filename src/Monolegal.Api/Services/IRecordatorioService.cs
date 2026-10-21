using Monolegal.Api.Models;

namespace Monolegal.Api.Services;

public interface IRecordatorioService
{
    Task<ResultadoProcesamiento> ProcesarAsync();
}
