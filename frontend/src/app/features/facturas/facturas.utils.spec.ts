import { FACTURAS_PRUEBA } from '../../testing/facturas-prueba';
import { filtrarFacturas, obtenerClientes, sumarMontos, totalizarPorEstado } from './facturas.utils';

describe('facturas.utils', () => {
  describe('filtrarFacturas', () => {
    it('sin filtros devuelve todas las facturas', () => {
      expect(filtrarFacturas(FACTURAS_PRUEBA, { clienteId: '', estado: '' })).toHaveLength(4);
    });

    it('filtra por cliente', () => {
      const resultado = filtrarFacturas(FACTURAS_PRUEBA, { clienteId: '900123456', estado: '' });
      expect(resultado.map((f) => f.numero)).toEqual(['FAC-1001', 'FAC-1002']);
    });

    it('filtra por estado', () => {
      const resultado = filtrarFacturas(FACTURAS_PRUEBA, { clienteId: '', estado: 'primerrecordatorio' });
      expect(resultado.map((f) => f.numero)).toEqual(['FAC-1001', 'FAC-3001']);
    });

    it('combina cliente y estado', () => {
      const resultado = filtrarFacturas(FACTURAS_PRUEBA, { clienteId: '900123456', estado: 'pagada' });
      expect(resultado.map((f) => f.numero)).toEqual(['FAC-1002']);
    });
  });

  it('obtenerClientes devuelve clientes únicos ordenados por nombre', () => {
    expect(obtenerClientes(FACTURAS_PRUEBA)).toEqual([
      { id: '800987654', nombre: 'Caribe Ltda' },
      { id: '900123456', nombre: 'Inversiones Andinas' },
      { id: '1020304050', nombre: 'Laura Gómez' },
    ]);
  });

  it('totalizarPorEstado cuenta y suma cada estado, incluidos los vacíos', () => {
    expect(totalizarPorEstado(FACTURAS_PRUEBA)).toEqual([
      { estado: 'primerrecordatorio', cantidad: 2, monto: 5000 },
      { estado: 'segundorecordatorio', cantidad: 1, monto: 3000 },
      { estado: 'desactivado', cantidad: 0, monto: 0 },
      { estado: 'pagada', cantidad: 1, monto: 2000 },
    ]);
  });

  it('sumarMontos suma el monto de todas las facturas', () => {
    expect(sumarMontos(FACTURAS_PRUEBA)).toBe(10000);
  });
});
