import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { FacturaResumen, ResultadoProcesamiento } from '../models/factura';

@Injectable({ providedIn: 'root' })
export class FacturasApi {
  private readonly http = inject(HttpClient);

  obtenerResumen(): Observable<FacturaResumen[]> {
    return this.http.get<FacturaResumen[]>('/api/facturas');
  }

  procesarRecordatorios(): Observable<ResultadoProcesamiento> {
    return this.http.post<ResultadoProcesamiento>('/api/recordatorios/procesar', null);
  }
}
