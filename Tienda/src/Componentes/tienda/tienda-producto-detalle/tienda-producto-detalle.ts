import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ProductoService } from '../../../app/services/producto';
import { MarcaService } from '../../../app/services/marca';
import { SubcategoriaService } from '../../../app/services/subcategoria';
import { CategoriaService } from '../../../app/services/categoria';
import { ImagenProductoService } from '../../../app/services/imagen-producto';
import { DescuentoService } from '../../../app/services/descuento';
import { ProductoDescuentoService } from '../../../app/services/producto-descuento';
import { InventarioService } from '../../../app/services/inventario';
import { BodegaService } from '../../../app/services/bodega';
import { IProducto } from '../../../app/model/IProducto';
import { IImagenProducto } from '../../../app/model/IImagenProducto';
import { IDescuento } from '../../../app/model/IDescuento';
import { IInventario } from '../../../app/model/IInventario';
import { IBodega } from '../../../app/model/IBodega';
import { urlImagen } from '../../../app/Utilitarios/ImagenUtils';
import { EspecificacionProductoService } from '../../../app/services/especificacion-producto';
import { GarantiaService } from '../../../app/services/garantia';
import { ProductoGarantiaService } from '../../../app/services/producto-garantia';
import { IEspecificacionProducto } from '../../../app/model/IEspecificacionProducto';

interface DisponibilidadTienda {
  bodegaNombre: string;
  cantidad: number;
}

@Component({
  selector: 'app-tienda-producto-detalle',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './tienda-producto-detalle.html',
  styleUrl: './tienda-producto-detalle.scss'
})
export class TiendaProductoDetalleComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private productoService = inject(ProductoService);
  private marcaService = inject(MarcaService);
  private subcategoriaService = inject(SubcategoriaService);
  private categoriaService = inject(CategoriaService);
  private imagenProductoService = inject(ImagenProductoService);
  private descuentoService = inject(DescuentoService);
  private productoDescuentoService = inject(ProductoDescuentoService);
  private inventarioService = inject(InventarioService);
  private bodegaService = inject(BodegaService);
  private especificacionService = inject(EspecificacionProductoService);
  private garantiaService = inject(GarantiaService);
  private productoGarantiaService = inject(ProductoGarantiaService);

  especificaciones = signal<IEspecificacionProducto[]>([]);
  garantiaMeses = signal<number | null>(null);

  cargando = signal(true);
  producto = signal<IProducto | null>(null);
  marcaNombre = signal('');
  categoriaNombre = signal('');
  categoriaId = signal<number | null>(null);
  subcategoriaNombre = signal('');

  imagenes = signal<string[]>([]);
  imagenActiva = signal('');

  precioOriginal = signal(0);
  precioFinal = signal(0);
  porcentajeDescuento = signal(0);

  disponibilidad = signal<DisponibilidadTienda[]>([]);
  mostrarDisponibilidad = signal(false);

  cantidad = signal(1);

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const productoId = Number(params.get('id'));
      if (productoId) {
        this.cargarProducto(productoId);
      }
    });
  }

  private cargarProducto(productoId: number): void {
    this.cargando.set(true);

    this.productoService.obtener(productoId).subscribe({
      next: respProd => {
        const prod: IProducto | undefined = respProd?.data;

        if (!prod) {
          this.cargando.set(false);
          this.producto.set(null);
          return;
        }

        this.producto.set(prod);
        this.precioOriginal.set(prod.precio);
        this.precioFinal.set(prod.precio);

        forkJoin({
          marca: this.marcaService.obtener(prod.marcaId)
            .pipe(catchError(() => of({ data: null }))),
          subcategoria: this.subcategoriaService.obtener(prod.subcategoriaId)
            .pipe(catchError(() => of({ data: null }))),
          imagenes: this.imagenProductoService.listarPorProducto(productoId)
            .pipe(catchError(() => of({ data: [] }))),
          descuentos: this.descuentoService.listar()
            .pipe(catchError(() => of({ data: [] }))),
          descuentosProducto: this.productoDescuentoService.listarPorProducto(productoId)
            .pipe(catchError(() => of({ data: [] }))),
          inventario: this.inventarioService.listarPorProducto(productoId)
            .pipe(catchError(() => of({ data: [] }))),
          bodegas: this.bodegaService.listar()
            .pipe(catchError(() => of({ data: [] }))),
            especificaciones: this.especificacionService.listarPorProducto(productoId)
            .pipe(catchError(() => of({ data: [] }))),
          garantiasProducto: this.productoGarantiaService.listarPorProducto(productoId)
            .pipe(catchError(() => of({ data: [] })))
        }).subscribe({
          next: ({ marca, subcategoria, imagenes, descuentos, descuentosProducto, inventario, bodegas, especificaciones, garantiasProducto }) => {

            this.marcaNombre.set(marca.data?.nombre ?? '');
            this.subcategoriaNombre.set(subcategoria.data?.nombre ?? '');
            this.especificaciones.set(especificaciones.data ?? []);

            const primeraGarantia = (garantiasProducto.data ?? [])[0];
            if (primeraGarantia) {
              this.garantiaService.obtener(primeraGarantia.garantiaId).subscribe({
                next: respGarantia => this.garantiaMeses.set(respGarantia.data?.meses ?? null),
                error: () => { }
              });
            }

            if (subcategoria.data?.categoriaId) {
              this.categoriaService.obtener(subcategoria.data.categoriaId).subscribe({
                next: respCat => {
                  this.categoriaNombre.set(respCat.data?.nombre ?? '');
                  this.categoriaId.set(subcategoria.data.categoriaId);
                },
                error: () => { }
              });
            }

            const rutasImagenes: IImagenProducto[] = imagenes.data ?? [];
            const urls = rutasImagenes.map(img => urlImagen(img.rutaImagen));
            this.imagenes.set(urls);
            this.imagenActiva.set(urls[0] ?? '');

            const descuentosData: IDescuento[] = descuentos.data ?? [];
            const hoy = new Date();
            const descuentoActivo = (descuentosProducto.data ?? [])
              .map((pd: any) => descuentosData.find(d => d.descuentoId === pd.descuentoId))
              .find((d?: IDescuento) =>
                d &&
                (!d.fechaInicio || new Date(d.fechaInicio) <= hoy) &&
                (!d.fechaFin || new Date(d.fechaFin) >= hoy)
              );

            if (descuentoActivo) {
              const porcentaje = Number(descuentoActivo.porcentaje);
              this.porcentajeDescuento.set(porcentaje);
              this.precioFinal.set(Math.round(prod.precio * (1 - porcentaje / 100)));
            }

            const inventarioData: IInventario[] = inventario.data ?? [];
            const bodegasData: IBodega[] = bodegas.data ?? [];
            this.disponibilidad.set(
              inventarioData
                .filter(i => i.cantidad > 0)
                .map(i => ({
                  bodegaNombre: bodegasData.find(b => b.bodegaId === i.bodegaId)?.nombre ?? 'Bodega',
                  cantidad: i.cantidad
                }))
            );

            this.cargando.set(false);
          },
          error: () => {
            this.cargando.set(false);
          }
        });
      },
      error: () => {
        this.cargando.set(false);
        this.producto.set(null);
      }
    });
  }

  cambiarImagenActiva(url: string): void {
    this.imagenActiva.set(url);
  }

  toggleDisponibilidad(): void {
    this.mostrarDisponibilidad.set(!this.mostrarDisponibilidad());
  }

  restarCantidad(): void {
    if (this.cantidad() > 1) this.cantidad.set(this.cantidad() - 1);
  }

  sumarCantidad(): void {
    this.cantidad.set(this.cantidad() + 1);
  }
}
