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
import { ResenaService } from '../../../app/services/resena';
import { IResena } from '../../../app/model/IResena';
import { CarritoService } from '../../../app/services/carrito';
import { IItemCarrito } from '../../../app/model/IItemCarrito';

// Datos de stock de una bodega/tienda para mostrar en "disponibilidad"
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
  // Servicios inyectados
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
  private resenaService = inject(ResenaService);
  private carritoService = inject(CarritoService);

  // Especificaciones técnicas del producto
  especificaciones = signal<IEspecificacionProducto[]>([]);
  // Meses de garantía del producto (si tiene)
  garantiaMeses = signal<number | null>(null);

  // Estado de carga del producto
  cargando = signal(true);
  // Producto actual
  producto = signal<IProducto | null>(null);
  marcaNombre = signal('');
  categoriaNombre = signal('');
  categoriaId = signal<number | null>(null);
  subcategoriaNombre = signal('');

  // Imágenes del producto y cuál está seleccionada
  imagenes = signal<string[]>([]);
  imagenActiva = signal('');

  // Precios calculados
  precioOriginal = signal(0);
  precioFinal = signal(0);
  porcentajeDescuento = signal(0);

  // Disponibilidad en tiendas/bodegas
  disponibilidad = signal<DisponibilidadTienda[]>([]);
  mostrarDisponibilidad = signal(false);

  // Cantidad a agregar al carrito
  cantidad = signal(1);

  // Reseñas del producto y su promedio
  resenas = signal<IResena[]>([]);
  promedioCalificacion = signal(0);

  ngOnInit(): void {
    // Escucha el id del producto en la URL y carga sus datos
    this.route.paramMap.subscribe(params => {
      const productoId = Number(params.get('id'));
      if (productoId) {
        this.cargarProducto(productoId);
      }
    });
  }

  private cargarProducto(productoId: number): void {
    this.cargando.set(true);

    // Primero obtiene el producto base
    this.productoService.obtener(productoId).subscribe({
      next: respProd => {
        const prod: IProducto | undefined = respProd?.data;

        // Si no existe el producto, detiene la carga
        if (!prod) {
          this.cargando.set(false);
          this.producto.set(null);
          return;
        }

        this.producto.set(prod);
        this.precioOriginal.set(prod.precio);
        this.precioFinal.set(prod.precio);

        // Trae toda la información relacionada al producto en paralelo
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
            .pipe(catchError(() => of({ data: [] }))),
          resenasProducto: this.resenaService.listarPorProducto(productoId)
            .pipe(catchError(() => of({ data: [] })))
        }).subscribe({
          next: ({ marca, subcategoria, imagenes, descuentos, descuentosProducto, inventario, bodegas, especificaciones, garantiasProducto, resenasProducto }) => {

            this.marcaNombre.set(marca.data?.nombre ?? '');
            this.subcategoriaNombre.set(subcategoria.data?.nombre ?? '');
            this.especificaciones.set(especificaciones.data ?? []);

            // Si el producto tiene garantía, obtiene sus meses
            const primeraGarantia = (garantiasProducto.data ?? [])[0];
            if (primeraGarantia) {
              this.garantiaService.obtener(primeraGarantia.garantiaId).subscribe({
                next: respGarantia => this.garantiaMeses.set(respGarantia.data?.meses ?? null),
                error: () => { }
              });
            }

            // Obtiene el nombre y id de la categoría a partir de la subcategoría
            if (subcategoria.data?.categoriaId) {
              this.categoriaService.obtener(subcategoria.data.categoriaId).subscribe({
                next: respCat => {
                  this.categoriaNombre.set(respCat.data?.nombre ?? '');
                  this.categoriaId.set(subcategoria.data.categoriaId);
                },
                error: () => { }
              });
            }

            // Arma las URLs de las imágenes y selecciona la primera como activa
            const rutasImagenes: IImagenProducto[] = imagenes.data ?? [];
            const urls = rutasImagenes.map(img => urlImagen(img.rutaImagen));
            this.imagenes.set(urls);
            this.imagenActiva.set(urls[0] ?? '');

            // Busca un descuento vigente para el producto y calcula el precio final
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

            // Arma la disponibilidad por bodega, solo con las que tienen stock
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

            // Guarda las reseñas y calcula el promedio de calificación
            const listaResenas: IResena[] = resenasProducto.data ?? [];
            this.resenas.set(listaResenas);
            this.promedioCalificacion.set(
              listaResenas.length > 0
                ? listaResenas.reduce((suma, r) => suma + r.calificacion, 0) / listaResenas.length
                : 0
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

  // Cambia la imagen principal mostrada en la galería
  cambiarImagenActiva(url: string): void {
    this.imagenActiva.set(url);
  }

  // Muestra u oculta el bloque de disponibilidad en tiendas
  toggleDisponibilidad(): void {
    this.mostrarDisponibilidad.set(!this.mostrarDisponibilidad());
  }

  // Disminuye la cantidad, sin bajar de 1
  restarCantidad(): void {
    if (this.cantidad() > 1) this.cantidad.set(this.cantidad() - 1);
  }

  // Aumenta la cantidad
  sumarCantidad(): void {
    this.cantidad.set(this.cantidad() + 1);
  }

  // Agrega el producto actual al carrito con la cantidad seleccionada
  agregarAlCarrito(): void {
    const prod = this.producto();
    if (!prod) return;

    const item: IItemCarrito = {
      productoId: prod.productoId,
      nombre: prod.nombre,
      precioUnitario: this.precioFinal(),
      cantidad: this.cantidad(),
      imagenUrl: this.imagenActiva()
    };

    this.carritoService.agregar(item);
  }

  // Convierte la calificación numérica en estrellas para mostrar
  estrellas(calificacion: number): string {
    return '⭐'.repeat(Math.round(calificacion));
  }
}
