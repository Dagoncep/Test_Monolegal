import { Component, input, output } from '@angular/core';
import { ResultadoProcesamiento } from '../../../core/models/factura';
import { EstadoBadge } from '../../../shared/estado-badge/estado-badge';

@Component({
  selector: 'app-resultado-procesamiento',
  imports: [EstadoBadge],
  templateUrl: './resultado-procesamiento.html',
  styleUrl: './resultado-procesamiento.scss',
})
export class ResultadoProcesamientoPanel {
  readonly resultado = input.required<ResultadoProcesamiento>();
  readonly cerrar = output<void>();
}
