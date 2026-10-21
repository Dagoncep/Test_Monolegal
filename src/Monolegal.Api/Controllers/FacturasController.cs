using Microsoft.AspNetCore.Mvc;
using Monolegal.Api.Models;
using Monolegal.Api.Services;

namespace Monolegal.Api.Controllers;

[ApiController]
[Route("api/facturas")]
public class FacturasController : ControllerBase
{
    private readonly IFacturaService _facturaService;

    public FacturasController(IFacturaService facturaService)
    {
        _facturaService = facturaService;
    }

    // GET /api/facturas: resumen de todas las facturas con los datos de su cliente
    [HttpGet]
    public async Task<ActionResult<List<FacturaResumen>>> ObtenerResumen()
    {
        return Ok(await _facturaService.ObtenerResumenAsync());
    }
}
