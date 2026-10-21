import { ESTADOS_FACTURA } from '../../core/models/estado-factura';
import { EstadoFactura, FacturaResumen } from '../../core/models/factura';

export interface FiltroFacturas {
  clienteId: string;
  estado: EstadoFactura | '';
}

export interface ClienteOpcion {
  id: string;
  nombre: string;
}

export interface TotalPorEstado {
  estado: EstadoFactura;
  cantidad: number;
  monto: number;
}

export function filtrarFacturas(facturas: readonly FacturaResumen[], filtro: FiltroFacturas): FacturaResumen[] {
  return facturas.filter(
    (factura) =>
      (!filtro.clienteId || factura.clienteId === filtro.clienteId) &&
      (!filtro.estado || factura.estado === filtro.estado),
  );
}

export function obtenerClientes(facturas: readonly FacturaResumen[]): ClienteOpcion[] {
  const clientes = new Map<string, string>();
  for (const factura of facturas) {
    clientes.set(factura.clienteId, factura.clienteNombre);
  }

  return [...clientes]
    .map(([id, nombre]) => ({ id, nombre }))
    .sort((a, b) => a.nombre.localeCompare(b.nombre, 'es'));
}

export function totalizarPorEstado(facturas: readonly FacturaResumen[]): TotalPorEstado[] {
  return ESTADOS_FACTURA.map((estado) => {
    const delEstado = facturas.filter((factura) => factura.estado === estado);
    return {
      estado,
      cantidad: delEstado.length,
      monto: sumarMontos(delEstado),
    };
  });
}

export function sumarMontos(facturas: readonly FacturaResumen[]): number {
  return facturas.reduce((total, factura) => total + factura.monto, 0);
}
