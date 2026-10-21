import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';
import { FACTURAS_PRUEBA } from '../../testing/facturas-prueba';
import { ResultadoProcesamiento } from '../models/factura';
import { FacturasApi } from './facturas-api';

describe('FacturasApi', () => {
  let api: FacturasApi;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    api = TestBed.inject(FacturasApi);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('obtenerResumen hace GET a /api/facturas', async () => {
    const respuesta = firstValueFrom(api.obtenerResumen());

    const peticion = http.expectOne('/api/facturas');
    expect(peticion.request.method).toBe('GET');
    peticion.flush(FACTURAS_PRUEBA);

    expect(await respuesta).toEqual(FACTURAS_PRUEBA);
  });

  it('procesarRecordatorios hace POST a /api/recordatorios/procesar', async () => {
    const resultado: ResultadoProcesamiento = { detalle: [], totalProcesadas: 0, totalExitosas: 0, totalFallidas: 0 };
    const respuesta = firstValueFrom(api.procesarRecordatorios());

    const peticion = http.expectOne('/api/recordatorios/procesar');
    expect(peticion.request.method).toBe('POST');
    peticion.flush(resultado);

    expect(await respuesta).toEqual(resultado);
  });
});
