import { CurrencyPipe } from '@angular/common';
import { Component, computed, input } from '@angular/core';
import { FacturaResumen } from '../../../core/models/factura';
import { EstadoBadge } from '../../../shared/estado-badge/estado-badge';
import { sumarMontos, totalizarPorEstado } from '../facturas.utils';

@Component({
  selector: 'app-resumen-facturas',
  imports: [CurrencyPipe, EstadoBadge],
  templateUrl: './resumen-facturas.html',
  styleUrl: './resumen-facturas.scss',
})
export class ResumenFacturas {
  readonly facturas = input.required<readonly FacturaResumen[]>();

  protected readonly montoTotal = computed(() => sumarMontos(this.facturas()));
  protected readonly totalesPorEstado = computed(() => totalizarPorEstado(this.facturas()));
}
