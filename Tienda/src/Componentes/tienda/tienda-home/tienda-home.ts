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

interface ProductoTarjeta extends IProducto {
  marcaNombre: string;
  precioOriginal: number;
  precioFinal: number;
  porcentajeDescuento: number;
  imagenUrl: string;
}

interface BannerItem {
  imagen: string;
  productoId: number;
}

const CANTIDAD_DESTACADOS = 8;
const INTERVALO_BANNER_MS = 4000;

@Component({
  selector: 'app-tienda-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './tienda-home.html',
  styleUrl: './tienda-home.scss'
})
export class TiendaHomeComponent implements OnInit, OnDestroy {
  private productoService = inject(ProductoService);
  private marcaService = inject(MarcaService);
  private descuentoService = inject(DescuentoService);
  private productoDescuentoService = inject(ProductoDescuentoService);
  private imagenProductoService = inject(ImagenProductoService);

  banners = signal<BannerItem[]>([
    { imagen: '/banner/banner1.png', productoId: 8 },
    { imagen: '/banner/banner2.png', productoId: 21 }
  ]);
  bannerActivo = signal(0);
  private intervaloBanner: any;

  cargandoDestacados = signal(true);
  productosDestacados = signal<ProductoTarjeta[]>([]);

  ngOnInit(): void {
    this.iniciarCarrusel();
    this.cargarDestacados();
  }

  ngOnDestroy(): void {
    clearInterval(this.intervaloBanner);
  }

  private iniciarCarrusel(): void {
    this.intervaloBanner = setInterval(() => this.siguienteBanner(), INTERVALO_BANNER_MS);
  }

  siguienteBanner(): void {
    const total = this.banners().length;
    this.bannerActivo.set((this.bannerActivo() + 1) % total);
  }

  irABanner(indice: number): void {
    this.bannerActivo.set(indice);
  }

  private cargarDestacados(): void {
    this.cargandoDestacados.set(true);

    forkJoin({
      productos: this.productoService.listar(),
      marcas: this.marcaService.listar(),
      descuentos: this.descuentoService.listar()
    }).subscribe({
      next: ({ productos, marcas, descuentos }) => {
        const todosLosProductos: IProducto[] = productos.data ?? [];
        const marcasData: IMarca[] = marcas.data ?? [];
        const descuentosData: IDescuento[] = descuentos.data ?? [];

        const seleccionados = todosLosProductos.slice(0, CANTIDAD_DESTACADOS);

        if (seleccionados.length === 0) {
          this.productosDestacados.set([]);
          this.cargandoDestacados.set(false);
          return;
        }

        const llamadasPorProducto = seleccionados.map(p =>
          forkJoin({
            descuentosProducto: this.productoDescuentoService.listarPorProducto(p.productoId)
              .pipe(catchError(() => of({ data: [] }))),
            imagenesProducto: this.imagenProductoService.listarPorProducto(p.productoId)
              .pipe(catchError(() => of({ data: [] })))
          }).pipe(
            map(({ descuentosProducto, imagenesProducto }) => {
              const hoy = new Date();

              const descuentoActivo = (descuentosProducto.data ?? [])
                .map((pd: any) => descuentosData.find(d => d.descuentoId === pd.descuentoId))
                .find((d?: IDescuento) =>
                  d &&
                  (!d.fechaInicio || new Date(d.fechaInicio) <= hoy) &&
                  (!d.fechaFin || new Date(d.fechaFin) >= hoy)
                );

              const porcentaje = descuentoActivo ? Number(descuentoActivo.porcentaje) : 0;
              const precioOriginal = p.precio;
              const precioFinal = porcentaje > 0
                ? Math.round(precioOriginal * (1 - porcentaje / 100))
                : precioOriginal;

              const primeraImagen = (imagenesProducto.data ?? [])[0];
              const imagenUrl = urlImagen(primeraImagen?.rutaImagen);

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

        forkJoin(llamadasPorProducto).subscribe(tarjetas => {
          this.productosDestacados.set(tarjetas);
          this.cargandoDestacados.set(false);
        });
      },
      error: () => {
        this.cargandoDestacados.set(false);
      }
    });
  }
}
