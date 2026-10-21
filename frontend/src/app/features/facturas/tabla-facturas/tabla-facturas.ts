import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, input } from '@angular/core';
import { FacturaResumen } from '../../../core/models/factura';
import { EstadoBadge } from '../../../shared/estado-badge/estado-badge';

@Component({
  selector: 'app-tabla-facturas',
  imports: [CurrencyPipe, DatePipe, EstadoBadge],
  templateUrl: './tabla-facturas.html',
  styleUrl: './tabla-facturas.scss',
})
export class TablaFacturas {
  readonly facturas = input.required<readonly FacturaResumen[]>();
}
