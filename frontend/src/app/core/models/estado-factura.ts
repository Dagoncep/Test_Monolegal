import { EstadoFactura } from './factura';

export const ESTADOS_FACTURA: readonly EstadoFactura[] = [
  'primerrecordatorio',
  'segundorecordatorio',
  'desactivado',
  'pagada',
];

const ETIQUETAS: Record<EstadoFactura, string> = {
  primerrecordatorio: 'Primer recordatorio',
  segundorecordatorio: 'Segundo recordatorio',
  desactivado: 'Desactivado',
  pagada: 'Pagada',
};

export function etiquetaEstado(estado: EstadoFactura): string {
  return ETIQUETAS[estado];
}
