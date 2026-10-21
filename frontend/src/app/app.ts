import { Component } from '@angular/core';
import { FacturasDashboard } from './features/facturas/facturas-dashboard/facturas-dashboard';

@Component({
  selector: 'app-root',
  imports: [FacturasDashboard],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {}
