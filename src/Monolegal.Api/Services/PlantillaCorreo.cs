using System.Globalization;
using System.Net;
using Monolegal.Api.Models;

namespace Monolegal.Api.Services;

// Arma el asunto y el cuerpo del correo según el estado actual de la factura
public static class PlantillaCorreo
{
    private static readonly CultureInfo Colombia = CultureInfo.GetCultureInfo("es-CO");

    public static string CrearAsunto(Factura factura)
    {
        return factura.Estado switch
        {
            EstadosFactura.PrimerRecordatorio => $"Segundo recordatorio de pago - Factura {factura.Numero}",
            EstadosFactura.SegundoRecordatorio => $"Aviso de desactivación del servicio - Factura {factura.Numero}",
            _ => throw new ArgumentException($"La factura {factura.Numero} no está en un estado de recordatorio.")
        };
    }

    public static string CrearCuerpo(Cliente cliente, Factura factura)
    {
        var mensaje = factura.Estado switch
        {
            EstadosFactura.PrimerRecordatorio =>
                $"Te informamos que tu factura {factura.Numero} continúa pendiente de pago y ha pasado a segundo recordatorio.",
            EstadosFactura.SegundoRecordatorio =>
                $"Tu factura {factura.Numero} sigue pendiente de pago después del segundo recordatorio, por lo que tu servicio va a ser desactivado.",
            _ => throw new ArgumentException($"La factura {factura.Numero} no está en un estado de recordatorio.")
        };

        // HtmlEncode evita que un nombre con caracteres como < o > rompa el HTML del correo
        return $"""
            <p>Hola {WebUtility.HtmlEncode(cliente.Nombre)},</p>
            <p>{mensaje}</p>
            <ul>
              <li><strong>Factura:</strong> {factura.Numero}</li>
              <li><strong>Valor:</strong> {factura.Monto.ToString("C0", Colombia)}</li>
              <li><strong>Fecha de vencimiento:</strong> {factura.FechaVencimiento.ToString("dd/MM/yyyy", Colombia)}</li>
            </ul>
            <p>Si ya realizaste el pago, por favor ignora este mensaje.</p>
            <p>Equipo de Cartera</p>
            """;
    }
}
