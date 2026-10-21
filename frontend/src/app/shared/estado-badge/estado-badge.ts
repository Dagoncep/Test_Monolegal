import { Component, computed, input } from '@angular/core';
import { etiquetaEstado } from '../../core/models/estado-factura';
import { EstadoFactura } from '../../core/models/factura';

@Component({
  selector: 'app-estado-badge',
  template: `<span class="badge" [attr.data-estado]="estado()">{{ etiqueta() }}</span>`,
})
export class EstadoBadge {
  readonly estado = input.required<EstadoFactura>();

  protected readonly etiqueta = computed(() => etiquetaEstado(this.estado()));
}
