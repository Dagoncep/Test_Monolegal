namespace Monolegal.Api.Models;

// Resultado de procesar una factura
public class ResultadoFactura
{
    public string Numero { get; set; } = string.Empty;
    public string EstadoAnterior { get; set; } = string.Empty;
    public string? EstadoNuevo { get; set; }
    public bool Exitoso { get; set; }
    public string? Error { get; set; }
}

// Resultado de una ejecución completa del proceso de recordatorios
public class ResultadoProcesamiento
{
    public List<ResultadoFactura> Detalle { get; set; } = [];

    public int TotalProcesadas => Detalle.Count;
    public int TotalExitosas => Detalle.Count(r => r.Exitoso);
    public int TotalFallidas => Detalle.Count(r => !r.Exitoso);
}
