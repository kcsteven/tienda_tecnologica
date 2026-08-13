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
import { FavoritosService } from '../../../app/services/favoritos';

// Producto con los datos ya calculados para mostrar en la tarjeta
interface ProductoTarjeta extends IProducto {
  marcaNombre: string;
  precioOriginal: number;
  precioFinal: number;
  porcentajeDescuento: number;
  etiquetas: string[];
  imagenUrl: string;
}

// Tiempo mínimo (en ms) que se muestra el spinner de carga, para que no sea un parpadeo
const TIEMPO_MINIMO_CARGA_MS = 800;

@Component({
  selector: 'app-tienda-producto-listado',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './tienda-producto-listado.html',
  styleUrl: './tienda-producto-listado.scss'
})
export class TiendaProductoListadoComponent implements OnInit {
  // Servicios inyectados
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

  favoritosService = inject(FavoritosService);

  // Nombre mostrado como título (categoría, subcategoría o término buscado)
  categoriaNombre = signal('');
  // Lista de productos sin ordenar ni paginar
  productosBase = signal<ProductoTarjeta[]>([]);
  cargando = signal(true);
  sinResultados = signal(false);

  // Configuración de paginación y orden
  cantidadPorPagina = signal(9);
  ordenPrecio = signal<'asc' | 'desc'>('asc');
  paginaActual = signal(1);

  // Momento (timestamp) en el que empezó la carga actual, para medir el tiempo mínimo del spinner
  private inicioCarga = 0;

  // Productos ordenados por precio según el orden seleccionado
  productosOrdenados = computed(() => {
    const productos = [...this.productosBase()];
    productos.sort((a, b) =>
      this.ordenPrecio() === 'asc'
        ? a.precioFinal - b.precioFinal
        : b.precioFinal - a.precioFinal
    );
    return productos;
  });

  // Productos que corresponden a la página actual
  productosPaginados = computed(() => {
    const inicio = (this.paginaActual() - 1) * this.cantidadPorPagina();
    return this.productosOrdenados().slice(inicio, inicio + this.cantidadPorPagina());
  });

  // Cantidad total de páginas según la cantidad por página elegida
  totalPaginas = computed(() =>
    Math.max(1, Math.ceil(this.productosOrdenados().length / this.cantidadPorPagina()))
  );

  ngOnInit(): void {
    // Revisa el tipo de listado según la data de la ruta (categoría, subcategoría o búsqueda)
    this.route.data.subscribe(data => {
      const tipo = data['tipo'];

      if (tipo === 'busqueda') {
        // Carga productos según el término de búsqueda en la URL
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

  toggleFavorito(event: Event, prod: ProductoTarjeta): void {
    event.preventDefault();
    event.stopPropagation();
    this.favoritosService.toggle({
      productoId: prod.productoId,
      nombre: prod.nombre,
      precioUnitario: prod.precioFinal,
      imagenUrl: prod.imagenUrl
    });
  }
  // Cambia cuántos productos se muestran por página y reinicia a la página 1
  cambiarCantidad(cantidad: number): void {
    this.cantidadPorPagina.set(cantidad);
    this.paginaActual.set(1);
  }

  // Cambia el orden de precio (ascendente o descendente)
  cambiarOrden(orden: 'asc' | 'desc'): void {
    this.ordenPrecio.set(orden);
  }

  // Marca el inicio de una carga: guarda la hora y prende el spinner
  private iniciarCarga(): void {
    this.inicioCarga = Date.now();
    this.cargando.set(true);
  }

  // Aplica los datos ya cargados, pero espera lo que falte para cumplir el tiempo mínimo del spinner
  private finalizarCarga(accion: () => void): void {
    const transcurrido = Date.now() - this.inicioCarga;
    const restante = TIEMPO_MINIMO_CARGA_MS - transcurrido;

    if (restante > 0) {
      setTimeout(() => {
        accion();
        this.cargando.set(false);
      }, restante);
    } else {
      accion();
      this.cargando.set(false);
    }
  }

  private cargarProductosPorCategoria(categoriaId: number): void {
    this.iniciarCarga();

    // Obtiene el nombre de la categoría
    this.categoriaService.obtener(categoriaId).subscribe(respCat => {
      this.categoriaNombre.set(respCat.data?.nombre ?? '');
    });

    // Trae todas las subcategorías de esta categoría, para filtrar productos por esos ids
    this.subcategoriaService.listarPorCategoria(categoriaId).subscribe({
      next: respSub => {
        const subcategoriaIds: number[] = (respSub.data ?? []).map((s: any) => s.subcategoriaId);
        this.cargarYFiltrarProductos(p => subcategoriaIds.includes(p.subcategoriaId));
      },
      error: () => {
        this.finalizarCarga(() => this.sinResultados.set(true));
      }
    });
  }

  private cargarProductosPorSubcategoria(subcategoriaId: number): void {
    this.iniciarCarga();

    // Obtiene el nombre de la subcategoría
    this.subcategoriaService.obtener(subcategoriaId).subscribe(respSub => {
      this.categoriaNombre.set(respSub.data?.nombre ?? '');
    });

    // Filtra los productos que pertenecen a esta subcategoría
    this.cargarYFiltrarProductos(p => p.subcategoriaId === subcategoriaId);
  }

  private cargarProductosPorBusqueda(termino: string): void {
    this.iniciarCarga();

    // Si no hay término de búsqueda, no hay resultados
    if (!termino) {
      this.finalizarCarga(() => {
        this.productosBase.set([]);
        this.sinResultados.set(true);
      });
      return;
    }

    // Busca productos por el término y trae marcas, descuentos y etiquetas en paralelo
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

        // Sin resultados de búsqueda
        if (productosEncontrados.length === 0) {
          this.finalizarCarga(() => {
            this.productosBase.set([]);
            this.sinResultados.set(true);
          });
          return;
        }

        this.sinResultados.set(false);

        // Por cada producto encontrado, trae sus descuentos, etiquetas e imágenes
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

              // Busca un descuento vigente para el producto
              const descuentoActivo = (descuentosProducto.data ?? [])
                .map((pd: any) => descuentosData.find(d => d.descuentoId === pd.descuentoId))
                .find((d?: IDescuento) =>
                  d &&
                  (!d.fechaInicio || new Date(d.fechaInicio) <= hoy) &&
                  (!d.fechaFin || new Date(d.fechaFin) >= hoy)
                );

              // Calcula el precio final según el porcentaje de descuento
              const porcentaje = descuentoActivo ? Number(descuentoActivo.porcentaje) : 0;
              const precioOriginal = p.precio;
              const precioFinal = porcentaje > 0
                ? Math.round(precioOriginal * (1 - porcentaje / 100))
                : precioOriginal;

              // Obtiene los nombres de las etiquetas del producto
              const nombresEtiquetas = (etiquetasProducto.data ?? [])
                .map((pe: any) => etiquetasData.find(e => e.etiquetaId === pe.etiquetaId)?.nombre)
                .filter((n: string | undefined): n is string => !!n);

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
                etiquetas: nombresEtiquetas,
                imagenUrl
              };
              return tarjeta;
            })
          )
        );

        // Espera a que todas las tarjetas estén listas y actualiza la lista
        forkJoin(llamadasPorProducto).subscribe(tarjetas => {
          this.finalizarCarga(() => {
            this.productosBase.set(tarjetas);
          });
        });
      },
      error: () => {
        this.finalizarCarga(() => this.sinResultados.set(true));
      }
    });
  }

  // Trae todos los productos y aplica un filtro (por categoría o subcategoría), luego arma las tarjetas
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

        // Aplica el filtro recibido (por categoría o subcategoría)
        const productosFiltrados = todosLosProductos.filter(filtro);

        // Sin productos que cumplan el filtro
        if (productosFiltrados.length === 0) {
          this.finalizarCarga(() => {
            this.productosBase.set([]);
            this.sinResultados.set(true);
          });
          return;
        }

        this.sinResultados.set(false);

        // Por cada producto filtrado, trae sus descuentos, etiquetas e imágenes
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

              // Busca un descuento vigente para el producto
              const descuentoActivo = (descuentosProducto.data ?? [])
                .map((pd: any) => descuentosData.find(d => d.descuentoId === pd.descuentoId))
                .find((d?: IDescuento) =>
                  d &&
                  (!d.fechaInicio || new Date(d.fechaInicio) <= hoy) &&
                  (!d.fechaFin || new Date(d.fechaFin) >= hoy)
                );

              // Calcula el precio final según el porcentaje de descuento
              const porcentaje = descuentoActivo ? Number(descuentoActivo.porcentaje) : 0;
              const precioOriginal = p.precio;
              const precioFinal = porcentaje > 0
                ? Math.round(precioOriginal * (1 - porcentaje / 100))
                : precioOriginal;

              // Obtiene los nombres de las etiquetas del producto
              const nombresEtiquetas = (etiquetasProducto.data ?? [])
                .map((pe: any) => etiquetasData.find(e => e.etiquetaId === pe.etiquetaId)?.nombre)
                .filter((n: string | undefined): n is string => !!n);

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
                etiquetas: nombresEtiquetas,
                imagenUrl
              };
              return tarjeta;
            })
          )
        );

        // Espera a que todas las tarjetas estén listas y actualiza la lista
        forkJoin(llamadasPorProducto).subscribe(tarjetas => {
          this.finalizarCarga(() => {
            this.productosBase.set(tarjetas);
          });
        });
      },
      error: () => {
        this.finalizarCarga(() => this.sinResultados.set(true));
      }
    });
  }
}
