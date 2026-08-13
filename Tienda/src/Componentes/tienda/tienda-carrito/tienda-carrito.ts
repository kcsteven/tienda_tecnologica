import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { CarritoService } from '../../../app/services/carrito';
import { MetodoPagoService } from '../../../app/services/metodo-pago';
import { PagoService } from '../../../app/services/pago';
import { IMetodoPago } from '../../../app/model/IMetodoPago';
import { IPedidoCrear } from '../../../app/model/IPedido';
import { PedidoService } from '../../../app/services/pedido';
import { AccesoUsuarioService } from '../../../app/services/Usuarios/acceso-usuario.service';
import { UltimaCompraService } from '../../../app/services/ultima-compra';

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
  private pedidoService = inject(PedidoService);
  private router = inject(Router);
  private accesoUsuarioService = inject(AccesoUsuarioService);
  private ultimaCompraService = inject(UltimaCompraService);

  clienteId: number | null = null;
  direccionId: number | null = null;

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
    this.clienteId = this.accesoUsuarioService.obtenerSesion()?.clienteId ?? null;
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
    if (this.carritoService.items().length === 0) {
      alert('El carrito está vacío.');
      return;
    }

    if (!this.metodoPagoSeleccionado) {
      alert('Selecciona un método de pago.');
      return;
    }

    if (!this.clienteId) {
      alert('No se pudo identificar tu cuenta de cliente. Cierra sesión y vuelve a iniciar sesión.');
      return;
    }

    this.procesandoPago = true;

    // Armamos la lista de detalles
    const detalles = this.carritoService.items().map(item => ({
      productoId: item.productoId,
      cantidad: item.cantidad
    }));

    // Objeto DTO que espera el endpoint /CrearCompra
    const pedido: IPedidoCrear = {
      clienteId: this.clienteId,
      direccionId: this.direccionId ?? null,
      metodoPagoId: this.metodoPagoSeleccionado,
      detalles: detalles
    };

    // UNA SOLA petición que procesa Pedido + Detalle + Pago en el Backend
    this.pedidoService.crearCompra(pedido).subscribe({
      next: (respuesta: any) => {
        const compra = respuesta.data;

        this.ultimaCompraService.guardar({
          pedidoId: compra.pedidoId,
          items: this.carritoService.items(),
          subtotal: this.subtotal,
          iva: this.iva,
          total: this.totalConIva
        });

        this.carritoService.vaciar();
        this.procesandoPago = false;

        alert(`¡Compra realizada con éxito! Pedido #${compra.pedidoId}`);

        this.router.navigate(['/pedido', compra.pedidoId]);
      },
      error: (error: any) => {
        console.error('Error procesando compra:', error);
        this.procesandoPago = false;
        const mensaje = error?.error?.error ?? 'No fue posible procesar la compra.';
        alert(mensaje);
      }
    });
  }
}
