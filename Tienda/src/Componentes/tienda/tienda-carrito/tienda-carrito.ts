import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CarritoService } from '../../../app/services/carrito';

@Component({
  selector: 'app-tienda-carrito',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './tienda-carrito.html',
  styleUrl: './tienda-carrito.scss'
})
export class TiendaCarritoComponent {
  // Servicio que maneja el estado del carrito
  carritoService = inject(CarritoService);

  // Baja en 1 la cantidad de un producto
  restarCantidad(productoId: number, cantidadActual: number): void {
    this.carritoService.actualizarCantidad(productoId, cantidadActual - 1);
  }

  // Sube en 1 la cantidad de un producto
  sumarCantidad(productoId: number, cantidadActual: number): void {
    this.carritoService.actualizarCantidad(productoId, cantidadActual + 1);
  }

  // Elimina un producto del carrito
  quitarItem(productoId: number): void {
    this.carritoService.quitar(productoId);
  }

  // Elimina todos los productos del carrito
  vaciarCarrito(): void {
    this.carritoService.vaciar();
  }
}
