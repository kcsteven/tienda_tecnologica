import { Injectable, signal } from '@angular/core';
import { IItemCarrito } from '../model/IItemCarrito';

export interface IResumenCompra {
  pedidoId: number;
  items: IItemCarrito[];
  subtotal: number;
  iva: number;
  total: number;
}

@Injectable({ providedIn: 'root' })
export class UltimaCompraService {
  private resumenSignal = signal<IResumenCompra | null>(null);
  resumen = this.resumenSignal.asReadonly();

  guardar(resumen: IResumenCompra): void {
    this.resumenSignal.set(resumen);
  }
}
