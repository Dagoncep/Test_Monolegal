namespace Monolegal.Api.Models;

// Lo que el frontend muestra en la tabla: la factura junto con los datos de su cliente
public class FacturaResumen
{
    public string Numero { get; set; } = string.Empty;
    public string ClienteId { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;
    public string? ClienteEmail { get; set; }
    public decimal Monto { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
