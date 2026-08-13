import { Injectable, computed, effect, signal } from '@angular/core';
import { IItemFavorito } from '../model/IItemFavorito';

const CLAVE_STORAGE = 'tienda_favoritos';

function leerFavoritosGuardados(): IItemFavorito[] {
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
export class FavoritosService {
  private itemsSignal = signal<IItemFavorito[]>(leerFavoritosGuardados());

  items = this.itemsSignal.asReadonly();

  cantidadTotal = computed(() => this.itemsSignal().length);

  constructor() {
    effect(() => {
      localStorage.setItem(CLAVE_STORAGE, JSON.stringify(this.itemsSignal()));
    });
  }

  esFavorito(productoId: number): boolean {
    return this.itemsSignal().some(i => i.productoId === productoId);
  }

  agregar(item: IItemFavorito): void {
    if (this.esFavorito(item.productoId)) return;
    this.itemsSignal.set([...this.itemsSignal(), item]);
  }

  quitar(productoId: number): void {
    this.itemsSignal.set(
      this.itemsSignal().filter(i => i.productoId !== productoId)
    );
  }

  toggle(item: IItemFavorito): void {
    if (this.esFavorito(item.productoId)) {
      this.quitar(item.productoId);
    } else {
      this.agregar(item);
    }
  }

  vaciar(): void {
    this.itemsSignal.set([]);
  }
}
