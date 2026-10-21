import { registerLocaleData } from '@angular/common';
import localeEsCo from '@angular/common/locales/es-CO';
import { LOCALE_ID } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ResultadoProcesamiento } from '../../../core/models/factura';
import { FacturasApi } from '../../../core/services/facturas-api';
import { FACTURAS_PRUEBA } from '../../../testing/facturas-prueba';
import { FacturasDashboard } from './facturas-dashboard';

registerLocaleData(localeEsCo);

const RESULTADO: ResultadoProcesamiento = {
  detalle: [
    {
      numero: 'FAC-1001',
      estadoAnterior: 'primerrecordatorio',
      estadoNuevo: 'segundorecordatorio',
      exitoso: true,
      error: null,
    },
  ],
  totalProcesadas: 1,
  totalExitosas: 1,
  totalFallidas: 0,
};

describe('FacturasDashboard', () => {
  let fixture: ComponentFixture<FacturasDashboard>;
  let api: { obtenerResumen: ReturnType<typeof vi.fn>; procesarRecordatorios: ReturnType<typeof vi.fn> };

  async function crearComponente(): Promise<HTMLElement> {
    await TestBed.configureTestingModule({
      imports: [FacturasDashboard],
      providers: [
        { provide: FacturasApi, useValue: api },
        { provide: LOCALE_ID, useValue: 'es-CO' },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(FacturasDashboard);
    await fixture.whenStable();
    return fixture.nativeElement as HTMLElement;
  }

  function filasTabla(elemento: HTMLElement): string[] {
    return [...elemento.querySelectorAll('tbody tr td.numero')].map((celda) => celda.textContent!.trim());
  }

  beforeEach(() => {
    api = {
      obtenerResumen: vi.fn(() => of(FACTURAS_PRUEBA)),
      procesarRecordatorios: vi.fn(() => of(RESULTADO)),
    };
  });

  afterEach(() => vi.restoreAllMocks());

  it('muestra todas las facturas al cargar', async () => {
    const elemento = await crearComponente();

    expect(filasTabla(elemento)).toEqual(['FAC-1001', 'FAC-1002', 'FAC-2001', 'FAC-3001']);
    expect(elemento.querySelector('.contador')?.textContent).toContain('4 de 4');
  });

  it('filtra la tabla por estado', async () => {
    const elemento = await crearComponente();
    const selectEstado = elemento.querySelectorAll('select')[1] as HTMLSelectElement;

    selectEstado.value = 'primerrecordatorio';
    selectEstado.dispatchEvent(new Event('change'));
    await fixture.whenStable();

    expect(filasTabla(elemento)).toEqual(['FAC-1001', 'FAC-3001']);
  });

  it('muestra un error si la API no responde', async () => {
    api.obtenerResumen.mockReturnValue(throwError(() => new Error('sin conexión')));

    const elemento = await crearComponente();

    expect(elemento.querySelector('[role="alert"]')?.textContent).toContain('No fue posible cargar las facturas');
  });

  it('al confirmar, procesa los recordatorios, muestra el resultado y recarga las facturas', async () => {
    vi.spyOn(window, 'confirm').mockReturnValue(true);
    const elemento = await crearComponente();

    (elemento.querySelector('.boton--primario') as HTMLButtonElement).click();
    await fixture.whenStable();

    expect(api.procesarRecordatorios).toHaveBeenCalledTimes(1);
    expect(api.obtenerResumen).toHaveBeenCalledTimes(2);
    expect(elemento.querySelector('app-resultado-procesamiento')?.textContent).toContain('1 de 1 facturas notificadas');
  });

  it('si el usuario cancela la confirmación no procesa nada', async () => {
    vi.spyOn(window, 'confirm').mockReturnValue(false);
    const elemento = await crearComponente();

    (elemento.querySelector('.boton--primario') as HTMLButtonElement).click();
    await fixture.whenStable();

    expect(api.procesarRecordatorios).not.toHaveBeenCalled();
  });
});
