import { Component, computed, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ESTADOS_FACTURA, etiquetaEstado } from '../../../core/models/estado-factura';
import { EstadoFactura, FacturaResumen, ResultadoProcesamiento } from '../../../core/models/factura';
import { FacturasApi } from '../../../core/services/facturas-api';
import { filtrarFacturas, obtenerClientes } from '../facturas.utils';
import { ResultadoProcesamientoPanel } from '../resultado-procesamiento/resultado-procesamiento';
import { ResumenFacturas } from '../resumen-facturas/resumen-facturas';
import { TablaFacturas } from '../tabla-facturas/tabla-facturas';

const MENSAJE_CONFIRMACION =
  'Se enviará un correo a cada cliente con facturas en primer o segundo recordatorio y se actualizará su estado. ¿Deseas continuar?';

@Component({
  selector: 'app-facturas-dashboard',
  imports: [ResumenFacturas, TablaFacturas, ResultadoProcesamientoPanel],
  templateUrl: './facturas-dashboard.html',
  styleUrl: './facturas-dashboard.scss',
})
export class FacturasDashboard {
  private readonly api = inject(FacturasApi);

  protected readonly estados = ESTADOS_FACTURA;
  protected readonly etiquetaEstado = etiquetaEstado;

  protected readonly facturas = signal<FacturaResumen[]>([]);
  protected readonly cargando = signal(true);
  protected readonly procesando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly resultado = signal<ResultadoProcesamiento | null>(null);

  protected readonly filtroCliente = signal('');
  protected readonly filtroEstado = signal<EstadoFactura | ''>('');

  protected readonly clientes = computed(() => obtenerClientes(this.facturas()));
  protected readonly facturasFiltradas = computed(() =>
    filtrarFacturas(this.facturas(), { clienteId: this.filtroCliente(), estado: this.filtroEstado() }),
  );
  protected readonly hayFiltros = computed(() => this.filtroCliente() !== '' || this.filtroEstado() !== '');

  constructor() {
    void this.cargarFacturas();
  }

  async cargarFacturas(): Promise<void> {
    this.cargando.set(true);
    this.error.set(null);
    try {
      this.facturas.set(await firstValueFrom(this.api.obtenerResumen()));
    } catch {
      this.error.set('No fue posible cargar las facturas. Verifica que la API esté en ejecución.');
    } finally {
      this.cargando.set(false);
    }
  }

  async procesarRecordatorios(): Promise<void> {
    if (!window.confirm(MENSAJE_CONFIRMACION)) {
      return;
    }

    this.procesando.set(true);
    this.resultado.set(null);
    this.error.set(null);
    try {
      this.resultado.set(await firstValueFrom(this.api.procesarRecordatorios()));
      await this.cargarFacturas();
    } catch {
      this.error.set('Ocurrió un error al procesar los recordatorios. Intenta nuevamente.');
    } finally {
      this.procesando.set(false);
    }
  }

  protected cambiarFiltroCliente(evento: Event): void {
    this.filtroCliente.set((evento.target as HTMLSelectElement).value);
  }

  protected cambiarFiltroEstado(evento: Event): void {
    this.filtroEstado.set((evento.target as HTMLSelectElement).value as EstadoFactura | '');
  }

  protected limpiarFiltros(): void {
    this.filtroCliente.set('');
    this.filtroEstado.set('');
  }
}
