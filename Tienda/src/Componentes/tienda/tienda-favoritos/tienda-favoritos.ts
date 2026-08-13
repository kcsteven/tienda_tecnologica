import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FavoritosService } from '../../../app/services/favoritos';
import { CarritoService } from '../../../app/services/carrito';
import { IItemCarrito } from '../../../app/model/IItemCarrito';

@Component({
  selector: 'app-tienda-favoritos',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './tienda-favoritos.html',
  styleUrl: './tienda-favoritos.scss'
})
export class TiendaFavoritosComponent {
  favoritosService = inject(FavoritosService);
  private carritoService = inject(CarritoService);

  quitar(productoId: number): void {
    this.favoritosService.quitar(productoId);
  }

  agregarAlCarrito(item: {
    productoId: number;
    nombre: string;
    precioUnitario: number;
    imagenUrl: string;
  }): void {
    const itemCarrito: IItemCarrito = {
      productoId: item.productoId,
      nombre: item.nombre,
      precioUnitario: item.precioUnitario,
      cantidad: 1,
      imagenUrl: item.imagenUrl
    };
    this.carritoService.agregar(itemCarrito);
  }
}
