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
  carritoService = inject(CarritoService);

  restarCantidad(productoId: number, cantidadActual: number): void {
    this.carritoService.actualizarCantidad(productoId, cantidadActual - 1);
  }

  sumarCantidad(productoId: number, cantidadActual: number): void {
    this.carritoService.actualizarCantidad(productoId, cantidadActual + 1);
  }

  quitarItem(productoId: number): void {
    this.carritoService.quitar(productoId);
  }

  vaciarCarrito(): void {
    this.carritoService.vaciar();
  }
}
