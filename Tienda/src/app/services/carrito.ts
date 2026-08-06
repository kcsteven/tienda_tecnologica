import { Injectable, computed, effect, signal } from '@angular/core';
import { IItemCarrito } from '../model/IItemCarrito';

const CLAVE_STORAGE = 'tienda_carrito';

function leerCarritoGuardado(): IItemCarrito[] {
  try {
    const data = localStorage.getItem(CLAVE_STORAGE);
    return data ? JSON.parse(data) : [];
  } catch {
    return [];
  }
}

@Injectable({
  providedIn: 'root',
})
export class CarritoService {
  private itemsSignal = signal<IItemCarrito[]>(leerCarritoGuardado());

  items = this.itemsSignal.asReadonly();

  cantidadTotal = computed(() =>
    this.itemsSignal().reduce((acc, item) => acc + item.cantidad, 0)
  );

  total = computed(() =>
    this.itemsSignal().reduce((acc, item) => acc + item.precioUnitario * item.cantidad, 0)
  );

  constructor() {
    effect(() => {
      localStorage.setItem(CLAVE_STORAGE, JSON.stringify(this.itemsSignal()));
    });
  }

  agregar(item: IItemCarrito): void {
    const items = this.itemsSignal();
    const existente = items.find(i => i.productoId === item.productoId);

    if (existente) {
      this.itemsSignal.set(
        items.map(i =>
          i.productoId === item.productoId
            ? { ...i, cantidad: i.cantidad + item.cantidad }
            : i
        )
      );
    } else {
      this.itemsSignal.set([...items, item]);
    }
  }

  actualizarCantidad(productoId: number, cantidad: number): void {
    if (cantidad < 1) return;

    this.itemsSignal.set(
      this.itemsSignal().map(i =>
        i.productoId === productoId ? { ...i, cantidad } : i
      )
    );
  }

  quitar(productoId: number): void {
    this.itemsSignal.set(
      this.itemsSignal().filter(i => i.productoId !== productoId)
    );
  }

  vaciar(): void {
    this.itemsSignal.set([]);
  }
}
