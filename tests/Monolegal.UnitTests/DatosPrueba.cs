using Monolegal.Api.Models;

namespace Monolegal.UnitTests;

// Crea objetos de prueba con valores por defecto para no repetirlos en cada test
public static class DatosPrueba
{
    public static Cliente Cliente(string id = "900123456", string nombre = "Cliente de prueba", string? email = null)
    {
        return new Cliente { Id = id, Nombre = nombre, Email = email ?? $"{id}@correo.test" };
    }

    public static Factura Factura(
        string numero = "FAC-1",
        string clienteId = "900123456",
        string estado = EstadosFactura.PrimerRecordatorio,
        decimal monto = 100000m)
    {
        return new Factura
        {
            Numero = numero,
            ClienteId = clienteId,
            Estado = estado,
            Monto = monto,
            FechaEmision = new DateTime(2026, 9, 1),
            FechaVencimiento = new DateTime(2026, 9, 30)
        };
    }
}
