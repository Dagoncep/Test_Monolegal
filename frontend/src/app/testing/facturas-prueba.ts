import { FacturaResumen } from '../core/models/factura';

export function crearFactura(cambios: Partial<FacturaResumen> = {}): FacturaResumen {
  return {
    numero: 'FAC-1',
    clienteId: '900123456',
    clienteNombre: 'Cliente Uno',
    clienteEmail: 'cliente@correo.test',
    monto: 100000,
    estado: 'primerrecordatorio',
    fechaEmision: '2026-09-01T00:00:00Z',
    fechaVencimiento: '2026-09-30T00:00:00Z',
    fechaActualizacion: null,
    ...cambios,
  };
}

export const FACTURAS_PRUEBA: FacturaResumen[] = [
  crearFactura({ numero: 'FAC-1001', clienteId: '900123456', clienteNombre: 'Inversiones Andinas', monto: 1000, estado: 'primerrecordatorio' }),
  crearFactura({ numero: 'FAC-1002', clienteId: '900123456', clienteNombre: 'Inversiones Andinas', monto: 2000, estado: 'pagada' }),
  crearFactura({ numero: 'FAC-2001', clienteId: '1020304050', clienteNombre: 'Laura Gómez', monto: 3000, estado: 'segundorecordatorio' }),
  crearFactura({ numero: 'FAC-3001', clienteId: '800987654', clienteNombre: 'Caribe Ltda', monto: 4000, estado: 'primerrecordatorio' }),
];
