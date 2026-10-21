using Microsoft.AspNetCore.Mvc;
using Monolegal.Api.Models;
using Monolegal.Api.Services;

namespace Monolegal.Api.Controllers;

[ApiController]
[Route("api/recordatorios")]
public class RecordatoriosController : ControllerBase
{
    private readonly IRecordatorioService _recordatorioService;

    public RecordatoriosController(IRecordatorioService recordatorioService)
    {
        _recordatorioService = recordatorioService;
    }

    // POST /api/recordatorios/procesar: envía los correos y actualiza los estados
    [HttpPost("procesar")]
    public async Task<ActionResult<ResultadoProcesamiento>> Procesar()
    {
        return Ok(await _recordatorioService.ProcesarAsync());
    }
}
