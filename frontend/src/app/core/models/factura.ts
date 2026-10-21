// Valores tal como los guarda MongoDB y los devuelve la API
export type EstadoFactura = 'primerrecordatorio' | 'segundorecordatorio' | 'desactivado' | 'pagada';

export interface FacturaResumen {
  numero: string;
  clienteId: string;
  clienteNombre: string;
  clienteEmail: string | null;
  monto: number;
  estado: EstadoFactura;
  fechaEmision: string;
  fechaVencimiento: string;
  fechaActualizacion: string | null;
}

export interface ResultadoFactura {
  numero: string;
  estadoAnterior: EstadoFactura;
  estadoNuevo: EstadoFactura | null;
  exitoso: boolean;
  error: string | null;
}

export interface ResultadoProcesamiento {
  detalle: ResultadoFactura[];
  totalProcesadas: number;
  totalExitosas: number;
  totalFallidas: number;
}
