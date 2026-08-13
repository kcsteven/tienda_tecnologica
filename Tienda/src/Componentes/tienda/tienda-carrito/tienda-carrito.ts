import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { CarritoService } from '../../../app/services/carrito';
import { MetodoPagoService } from '../../../app/services/metodo-pago';
import { PagoService } from '../../../app/services/pago';
import { IMetodoPago } from '../../../app/model/IMetodoPago';

@Component({
  selector: 'app-tienda-carrito',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './tienda-carrito.html',
  styleUrl: './tienda-carrito.scss'
})
export class TiendaCarritoComponent implements OnInit {

  carritoService = inject(CarritoService);

  private metodoPagoService = inject(MetodoPagoService);
  private pagoService = inject(PagoService);
  private router = inject(Router);

  metodosPago: IMetodoPago[] = [];
  metodoPagoSeleccionado: number | null = null;

  procesandoPago = false;

  // Variables para controlar mensajes de error/validación en la interfaz
  mensajeError: string | null = null;
  mensajeExito: string | null = null;

  subtotal = 0;
  iva = 0;
  totalConIva = 0;

  ngOnInit(): void {
    this.calcularTotales();
    this.cargarMetodosPago();
  }

  calcularTotales(): void {
    this.subtotal = this.carritoService.total();
    this.iva = this.subtotal * 0.13;
    this.totalConIva = this.subtotal + this.iva;
  }

  cargarMetodosPago(): void {
    this.metodoPagoService.listar()
      .subscribe({
        next: (respuesta) => {
          this.metodosPago = respuesta.data ?? [];
        },
        error: (error) => {
          console.error('Error cargando métodos de pago', error);
          this.mensajeError = 'No se pudieron cargar los métodos de pago.';
        }
      });
  }

  restarCantidad(productoId: number, cantidadActual: number): void {
    if (cantidadActual <= 1) {
      return;
    }
    this.carritoService.actualizarCantidad(productoId, cantidadActual - 1);
    this.calcularTotales();
  }

  sumarCantidad(productoId: number, cantidadActual: number): void {
    this.carritoService.actualizarCantidad(productoId, cantidadActual + 1);
    this.calcularTotales();
  }

  quitarItem(productoId: number): void {
    this.carritoService.quitar(productoId);
    this.calcularTotales();
  }

  vaciarCarrito(): void {
    this.carritoService.vaciar();
    this.calcularTotales();
  }

  seleccionarMetodoPago(metodoPagoId: number): void {
    this.metodoPagoSeleccionado = metodoPagoId;
    this.mensajeError = null; // Limpia errores previos al seleccionar
  }

  finalizarCompra(): void {
    this.mensajeError = null;
    this.mensajeExito = null;

    if (this.carritoService.items().length === 0) {
      this.mensajeError = 'El carrito se encuentra vacío.';
      return;
    }

    if (!this.metodoPagoSeleccionado) {
      this.mensajeError = 'Por favor, selecciona un método de pago.';
      return;
    }

    /*
     * Aquí conectaremos la creación del Pedido y posterior Pago
     */
    this.procesandoPago = true;

    // Lógica para enviar la compra al backend...
  }
}
