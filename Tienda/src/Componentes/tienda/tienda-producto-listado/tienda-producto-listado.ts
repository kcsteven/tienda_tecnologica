import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { ProductoService } from '../../../app/services/producto';
import { SubcategoriaService } from '../../../app/services/subcategoria';
import { MarcaService } from '../../../app/services/marca';
import { CategoriaService } from '../../../app/services/categoria';
import { DescuentoService } from '../../../app/services/descuento';
import { ProductoDescuentoService } from '../../../app/services/producto-descuento';
import { EtiquetaService } from '../../../app/services/etiqueta';
import { ProductoEtiquetaService } from '../../../app/services/producto-etiqueta';
import { ImagenProductoService } from '../../../app/services/imagen-producto';
import { IProducto } from '../../../app/model/IProducto';
import { IMarca } from '../../../app/model/IMarca';
import { IDescuento } from '../../../app/model/IDescuento';
import { IEtiqueta } from '../../../app/model/IEtiqueta';
import { urlImagen } from '../../../app/Utilitarios/ImagenUtils';

interface ProductoTarjeta extends IProducto {
  marcaNombre: string;
  precioOriginal: number;
  precioFinal: number;
  porcentajeDescuento: number;
  etiquetas: string[];
  imagenUrl: string;
}

@Component({
  selector: 'app-tienda-producto-listado',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './tienda-producto-listado.html',
  styleUrl: './tienda-producto-listado.scss'
})
export class TiendaProductoListadoComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private categoriaService = inject(CategoriaService);
  private subcategoriaService = inject(SubcategoriaService);
  private productoService = inject(ProductoService);
  private marcaService = inject(MarcaService);
  private descuentoService = inject(DescuentoService);
  private productoDescuentoService = inject(ProductoDescuentoService);
  private etiquetaService = inject(EtiquetaService);
  private productoEtiquetaService = inject(ProductoEtiquetaService);
  private imagenProductoService = inject(ImagenProductoService);

  categoriaNombre = signal('');
  productosBase = signal<ProductoTarjeta[]>([]);
  cargando = signal(true);
  sinResultados = signal(false);

  cantidadPorPagina = signal(9);
  ordenPrecio = signal<'asc' | 'desc'>('asc');
  paginaActual = signal(1);

  productosOrdenados = computed(() => {
    const productos = [...this.productosBase()];
    productos.sort((a, b) =>
      this.ordenPrecio() === 'asc'
        ? a.precioFinal - b.precioFinal
        : b.precioFinal - a.precioFinal
    );
    return productos;
  });

  productosPaginados = computed(() => {
    const inicio = (this.paginaActual() - 1) * this.cantidadPorPagina();
    return this.productosOrdenados().slice(inicio, inicio + this.cantidadPorPagina());
  });

  totalPaginas = computed(() =>
    Math.max(1, Math.ceil(this.productosOrdenados().length / this.cantidadPorPagina()))
  );

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      const tipo = data['tipo'];

      if (tipo === 'busqueda') {
        this.route.queryParamMap.subscribe(params => {
          const termino = params.get('q') ?? '';
          this.categoriaNombre.set(`Resultados para "${termino}"`);
          this.cargarProductosPorBusqueda(termino);
        });
        return;
      }

      this.route.paramMap.subscribe(params => {
        const id = Number(params.get('id'));
        if (!id) return;

        if (tipo === 'subcategoria') {
          this.cargarProductosPorSubcategoria(id);
        } else {
          this.cargarProductosPorCategoria(id);
        }
      });
    });
  }

  cambiarCantidad(cantidad: number): void {
    this.cantidadPorPagina.set(cantidad);
    this.paginaActual.set(1);
  }

  cambiarOrden(orden: 'asc' | 'desc'): void {
    this.ordenPrecio.set(orden);
  }

  
  private cargarProductosPorCategoria(categoriaId: number): void {
    this.cargando.set(true);

    this.categoriaService.obtener(categoriaId).subscribe(respCat => {
      this.categoriaNombre.set(respCat.data?.nombre ?? '');
    });

    this.subcategoriaService.listarPorCategoria(categoriaId).subscribe({
      next: respSub => {
        const subcategoriaIds: number[] = (respSub.data ?? []).map((s: any) => s.subcategoriaId);
        this.cargarYFiltrarProductos(p => subcategoriaIds.includes(p.subcategoriaId));
      },
      error: () => {
        this.cargando.set(false);
        this.sinResultados.set(true);
      }
    });
  }

 
  private cargarProductosPorSubcategoria(subcategoriaId: number): void {
    this.cargando.set(true);

    this.subcategoriaService.obtener(subcategoriaId).subscribe(respSub => {
      this.categoriaNombre.set(respSub.data?.nombre ?? '');
    });

    this.cargarYFiltrarProductos(p => p.subcategoriaId === subcategoriaId);
  }

  private cargarProductosPorBusqueda(termino: string): void {
    this.cargando.set(true);

    if (!termino) {
      this.productosBase.set([]);
      this.cargando.set(false);
      this.sinResultados.set(true);
      return;
    }

    forkJoin({
      productos: this.productoService.buscar(termino),
      marcas: this.marcaService.listar(),
      descuentos: this.descuentoService.listar(),
      etiquetas: this.etiquetaService.listar()
    }).subscribe({
      next: ({ productos, marcas, descuentos, etiquetas }) => {
        const productosEncontrados: IProducto[] = productos.data ?? [];
        const marcasData: IMarca[] = marcas.data ?? [];
        const descuentosData: IDescuento[] = descuentos.data ?? [];
        const etiquetasData: IEtiqueta[] = etiquetas.data ?? [];

        if (productosEncontrados.length === 0) {
          this.productosBase.set([]);
          this.cargando.set(false);
          this.sinResultados.set(true);
          return;
        }

        this.sinResultados.set(false);

        const llamadasPorProducto = productosEncontrados.map(p =>
          forkJoin({
            descuentosProducto: this.productoDescuentoService.listarPorProducto(p.productoId)
              .pipe(catchError(() => of({ data: [] }))),
            etiquetasProducto: this.productoEtiquetaService.listarPorProducto(p.productoId)
              .pipe(catchError(() => of({ data: [] }))),
            imagenesProducto: this.imagenProductoService.listarPorProducto(p.productoId)
              .pipe(catchError(() => of({ data: [] })))
          }).pipe(
            map(({ descuentosProducto, etiquetasProducto, imagenesProducto }) => {
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

              const nombresEtiquetas = (etiquetasProducto.data ?? [])
                .map((pe: any) => etiquetasData.find(e => e.etiquetaId === pe.etiquetaId)?.nombre)
                .filter((n: string | undefined): n is string => !!n);

              const primeraImagen = (imagenesProducto.data ?? [])[0];
              const imagenUrl = urlImagen(primeraImagen?.rutaImagen);

              const tarjeta: ProductoTarjeta = {
                ...p,
                marcaNombre: marcasData.find(m => m.marcaId === p.marcaId)?.nombre ?? '',
                precioOriginal,
                precioFinal,
                porcentajeDescuento: porcentaje,
                etiquetas: nombresEtiquetas,
                imagenUrl
              };
              return tarjeta;
            })
          )
        );

        forkJoin(llamadasPorProducto).subscribe(tarjetas => {
          this.productosBase.set(tarjetas);
          this.cargando.set(false);
        });
      },
      error: () => {
        this.cargando.set(false);
        this.sinResultados.set(true);
      }
    });
  }

  private cargarYFiltrarProductos(filtro: (p: IProducto) => boolean): void {
    forkJoin({
      productos: this.productoService.listar(),
      marcas: this.marcaService.listar(),
      descuentos: this.descuentoService.listar(),
      etiquetas: this.etiquetaService.listar()
    }).subscribe({
      next: ({ productos, marcas, descuentos, etiquetas }) => {
        const todosLosProductos: IProducto[] = productos.data ?? [];
        const marcasData: IMarca[] = marcas.data ?? [];
        const descuentosData: IDescuento[] = descuentos.data ?? [];
        const etiquetasData: IEtiqueta[] = etiquetas.data ?? [];

        const productosFiltrados = todosLosProductos.filter(filtro);

        if (productosFiltrados.length === 0) {
          this.productosBase.set([]);
          this.cargando.set(false);
          this.sinResultados.set(true);
          return;
        }

        this.sinResultados.set(false);

        const llamadasPorProducto = productosFiltrados.map(p =>
          forkJoin({
            descuentosProducto: this.productoDescuentoService.listarPorProducto(p.productoId)
              .pipe(catchError(() => of({ data: [] }))),
            etiquetasProducto: this.productoEtiquetaService.listarPorProducto(p.productoId)
              .pipe(catchError(() => of({ data: [] }))),
            imagenesProducto: this.imagenProductoService.listarPorProducto(p.productoId)
              .pipe(catchError(() => of({ data: [] })))
          }).pipe(
            map(({ descuentosProducto, etiquetasProducto, imagenesProducto }) => {
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

              const nombresEtiquetas = (etiquetasProducto.data ?? [])
                .map((pe: any) => etiquetasData.find(e => e.etiquetaId === pe.etiquetaId)?.nombre)
                .filter((n: string | undefined): n is string => !!n);

              const primeraImagen = (imagenesProducto.data ?? [])[0];
              const imagenUrl = urlImagen(primeraImagen?.rutaImagen);

              const tarjeta: ProductoTarjeta = {
                ...p,
                marcaNombre: marcasData.find(m => m.marcaId === p.marcaId)?.nombre ?? '',
                precioOriginal,
                precioFinal,
                porcentajeDescuento: porcentaje,
                etiquetas: nombresEtiquetas,
                imagenUrl
              };
              return tarjeta;
            })
          )
        );

        forkJoin(llamadasPorProducto).subscribe(tarjetas => {
          this.productosBase.set(tarjetas);
          this.cargando.set(false);
        });
      },
      error: () => {
        this.cargando.set(false);
        this.sinResultados.set(true);
      }
    });
  }
}
