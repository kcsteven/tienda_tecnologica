import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ProductoService } from '../../../app/services/producto';
import { MarcaService } from '../../../app/services/marca';
import { DescuentoService } from '../../../app/services/descuento';
import { ProductoDescuentoService } from '../../../app/services/producto-descuento';
import { ImagenProductoService } from '../../../app/services/imagen-producto';
import { IProducto } from '../../../app/model/IProducto';
import { IMarca } from '../../../app/model/IMarca';
import { IDescuento } from '../../../app/model/IDescuento';
import { urlImagen } from '../../../app/Utilitarios/ImagenUtils';

// Producto con los datos ya calculados para mostrar en la tarjeta
interface ProductoTarjeta extends IProducto {
  marcaNombre: string;
  precioOriginal: number;
  precioFinal: number;
  porcentajeDescuento: number;
  imagenUrl: string;
}

// Estructura de cada banner del carrusel
interface BannerItem {
  imagen: string;
  productoId: number;
}

// Cantidad de productos destacados a mostrar
const CANTIDAD_DESTACADOS = 8;
// Tiempo entre cambios de banner (en milisegundos)
const INTERVALO_BANNER_MS = 4000;

@Component({
  selector: 'app-tienda-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './tienda-home.html',
  styleUrl: './tienda-home.scss'
})
export class TiendaHomeComponent implements OnInit, OnDestroy {
  // Servicios inyectados
  private productoService = inject(ProductoService);
  private marcaService = inject(MarcaService);
  private descuentoService = inject(DescuentoService);
  private productoDescuentoService = inject(ProductoDescuentoService);
  private imagenProductoService = inject(ImagenProductoService);

  // Lista de banners del carrusel
  banners = signal<BannerItem[]>([
    { imagen: '/banner/banner1.png', productoId: 8 },
    { imagen: '/banner/banner2.png', productoId: 21 }
  ]);
  // Índice del banner que se está mostrando
  bannerActivo = signal(0);
  // Referencia al intervalo del carrusel automático
  private intervaloBanner: any;

  // Indica si los productos destacados aún están cargando
  cargandoDestacados = signal(true);
  // Lista de productos destacados ya procesados
  productosDestacados = signal<ProductoTarjeta[]>([]);

  ngOnInit(): void {
    // Al iniciar el componente, arranca el carrusel y carga los productos
    this.iniciarCarrusel();
    this.cargarDestacados();
  }

  ngOnDestroy(): void {
    // Limpia el intervalo del carrusel al destruir el componente
    clearInterval(this.intervaloBanner);
  }

  private iniciarCarrusel(): void {
    // Cambia de banner automáticamente cada cierto tiempo
    this.intervaloBanner = setInterval(() => this.siguienteBanner(), INTERVALO_BANNER_MS);
  }

  siguienteBanner(): void {
    // Pasa al siguiente banner, volviendo al inicio si llega al final
    const total = this.banners().length;
    this.bannerActivo.set((this.bannerActivo() + 1) % total);
  }

  irABanner(indice: number): void {
    // Cambia directamente al banner indicado (por click en los puntos)
    this.bannerActivo.set(indice);
  }

  private cargarDestacados(): void {
    // Marca que se está cargando
    this.cargandoDestacados.set(true);

    // Trae productos, marcas y descuentos al mismo tiempo
    forkJoin({
      productos: this.productoService.listar(),
      marcas: this.marcaService.listar(),
      descuentos: this.descuentoService.listar()
    }).subscribe({
      next: ({ productos, marcas, descuentos }) => {
        const todosLosProductos: IProducto[] = productos.data ?? [];
        const marcasData: IMarca[] = marcas.data ?? [];
        const descuentosData: IDescuento[] = descuentos.data ?? [];

        // Toma solo la cantidad definida de productos destacados
        const seleccionados = todosLosProductos.slice(0, CANTIDAD_DESTACADOS);

        // Si no hay productos, termina la carga con lista vacía
        if (seleccionados.length === 0) {
          this.productosDestacados.set([]);
          this.cargandoDestacados.set(false);
          return;
        }

        // Por cada producto seleccionado, trae sus descuentos e imágenes
        const llamadasPorProducto = seleccionados.map(p =>
          forkJoin({
            descuentosProducto: this.productoDescuentoService.listarPorProducto(p.productoId)
              .pipe(catchError(() => of({ data: [] }))),
            imagenesProducto: this.imagenProductoService.listarPorProducto(p.productoId)
              .pipe(catchError(() => of({ data: [] })))
          }).pipe(
            map(({ descuentosProducto, imagenesProducto }) => {
              const hoy = new Date();

              // Busca un descuento vigente (dentro de fecha inicio y fin) para el producto
              const descuentoActivo = (descuentosProducto.data ?? [])
                .map((pd: any) => descuentosData.find(d => d.descuentoId === pd.descuentoId))
                .find((d?: IDescuento) =>
                  d &&
                  (!d.fechaInicio || new Date(d.fechaInicio) <= hoy) &&
                  (!d.fechaFin || new Date(d.fechaFin) >= hoy)
                );

              // Calcula el porcentaje y el precio final según si hay descuento activo
              const porcentaje = descuentoActivo ? Number(descuentoActivo.porcentaje) : 0;
              const precioOriginal = p.precio;
              const precioFinal = porcentaje > 0
                ? Math.round(precioOriginal * (1 - porcentaje / 100))
                : precioOriginal;

              // Toma la primera imagen del producto
              const primeraImagen = (imagenesProducto.data ?? [])[0];
              const imagenUrl = urlImagen(primeraImagen?.rutaImagen);

              // Arma el objeto final con todos los datos para la tarjeta
              const tarjeta: ProductoTarjeta = {
                ...p,
                marcaNombre: marcasData.find(m => m.marcaId === p.marcaId)?.nombre ?? '',
                precioOriginal,
                precioFinal,
                porcentajeDescuento: porcentaje,
                imagenUrl
              };
              return tarjeta;
            })
          )
        );

        // Espera a que todas las tarjetas estén listas y actualiza la lista
        forkJoin(llamadasPorProducto).subscribe(tarjetas => {
          this.productosDestacados.set(tarjetas);
          this.cargandoDestacados.set(false);
        });
      },
      error: () => {
        // Si falla la carga, deja de mostrar el estado de carga
        this.cargandoDestacados.set(false);
      }
    });
  }
}
