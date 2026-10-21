using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Monolegal.Api.Models;

public class Factura
{
    // Número de la factura (ej. FAC-1001); es el _id del documento en MongoDB
    [BsonId]
    public string Numero { get; set; } = string.Empty;

    // NIT o cédula del cliente dueño de la factura
    public string ClienteId { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Monto { get; set; }

    // Uno de los valores de EstadosFactura
    public string Estado { get; set; } = string.Empty;

    public DateTime FechaEmision { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public DateTime? FechaActualizacion { get; set; }
}
