import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { IBodega } from '../../app/model/IBodega';
import { ICategoria } from '../../app/model/ICategoria';
import { IInventario } from '../../app/model/IInventario';
import { IMarca } from '../../app/model/IMarca';
import { IProducto } from '../../app/model/IProducto';
import { IProveedor } from '../../app/model/IProveedor';
import { ISubcategoria } from '../../app/model/ISubcategoria';
import { BodegaService } from '../../app/services/bodega';
import { CategoriaService } from '../../app/services/categoria';
import { InventarioService } from '../../app/services/inventario';
import { MarcaService } from '../../app/services/marca';
import { ProductoService } from '../../app/services/producto';
import { ProveedorService } from '../../app/services/proveedor';
import { SubcategoriaService } from '../../app/services/subcategoria';

@Component({
  selector: 'app-producto',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './producto.html',
  styleUrl: './producto.scss',
})
export class ProductoComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);
  private readonly productoService = inject(ProductoService);
  private readonly inventarioService = inject(InventarioService);
  private readonly subcategoriaService = inject(SubcategoriaService);
  private readonly marcaService = inject(MarcaService);
  private readonly proveedorService = inject(ProveedorService);
  private readonly bodegaService = inject(BodegaService);
  private readonly categoriaService = inject(CategoriaService);

  productos: IProducto[] = [];
  inventarios: IInventario[] = [];
  subcategorias: ISubcategoria[] = [];
  marcas: IMarca[] = [];
  proveedores: IProveedor[] = [];
  bodegas: IBodega[] = [];
  categorias: ICategoria[] = [];
  inventariosProductoSeleccionado: IInventario[] = [];
  ajustesInventario: Record<number, number> = {};
  productoSeleccionado: IProducto | null = null;
  cargando = true;
  procesando = false;
  editando = false;
  mensajeError = '';
  mensajeExito = '';

  formulario = this.formBuilder.group({
    productoId: [0],
    nombre: ['', [Validators.required, Validators.maxLength(150)]],
    descripcion: ['', [Validators.maxLength(500)]],
    precio: [0, [Validators.required, Validators.min(0.01)]],
    costoCompra: [null as number | null, [Validators.min(0)]],
    subcategoriaId: [0, [Validators.required, Validators.min(1)]],
    marcaId: [0, [Validators.required, Validators.min(1)]],
    proveedorId: [0, [Validators.required, Validators.min(1)]],
    bodegaId: [0, [Validators.required, Validators.min(1)]],
    cantidadInicial: [0, [Validators.required, Validators.min(0)]],
  });

  filtros = this.formBuilder.group({
    nombre: [''],
    marcaId: [0],
    categoriaId: [0],
    subcategoriaId: [0],
  });

  ngOnInit(): void {
    this.cargarDatosIniciales();
  }

  cargarDatosIniciales(): void {
    this.cargando = true;
    this.mensajeError = '';

    // Los catálogos se cargan una sola vez y se reutilizan en el formulario.
    forkJoin({
      productos: this.productoService.listarAdministracion(),
      inventarios: this.inventarioService.listar(),
      subcategorias: this.subcategoriaService.listar(),
      marcas: this.marcaService.listar(),
      proveedores: this.proveedorService.listar(),
      bodegas: this.bodegaService.listar(),
      categorias: this.categoriaService.listar(),
    }).subscribe({
      next: (respuestas) => {
        if (Object.values(respuestas).some((respuesta: any) => !!respuesta?.error)) {
          this.mensajeError = 'No fue posible cargar la información de productos.';
          this.cargando = false;
          this.changeDetectorRef.markForCheck();
          return;
        }

        this.productos = respuestas.productos.data ?? [];
        this.inventarios = respuestas.inventarios.data ?? [];
        this.subcategorias = respuestas.subcategorias.data ?? [];
        this.marcas = respuestas.marcas.data ?? [];
        this.proveedores = respuestas.proveedores.data ?? [];
        this.bodegas = respuestas.bodegas.data ?? [];
        this.categorias = respuestas.categorias.data ?? [];
        this.actualizarInventariosSeleccionados();
        this.cargando = false;

        this.changeDetectorRef.markForCheck();
      },
      error: () => {
        this.mensajeError = 'No fue posible cargar la información de productos';
        this.cargando = false;

        this.changeDetectorRef.markForCheck();
      },
    });
  }

  guardar(): void {
    this.mensajeError = '';
    this.mensajeExito = '';
    this.formulario.markAllAsTouched();

    if (this.formulario.invalid || this.procesando) {
      return;
    }

    const valor = this.formulario.getRawValue();
    this.procesando = true;

    const solicitud = this.editando
      ? this.productoService.modificar({
          productoId: Number(valor.productoId),
          nombre: valor.nombre?.trim() ?? '',
          descripcion: valor.descripcion?.trim() || null,
          precio: Number(valor.precio),
          costoCompra: valor.costoCompra === null ? null : Number(valor.costoCompra),
          subcategoriaId: Number(valor.subcategoriaId),
          marcaId: Number(valor.marcaId),
          proveedorId: Number(valor.proveedorId),
        })
      : this.productoService.insertar({
          nombre: valor.nombre?.trim() ?? '',
          descripcion: valor.descripcion?.trim() || null,
          precio: Number(valor.precio),
          costoCompra: valor.costoCompra === null ? null : Number(valor.costoCompra),
          subcategoriaId: Number(valor.subcategoriaId),
          marcaId: Number(valor.marcaId),
          proveedorId: Number(valor.proveedorId),
          bodegaId: Number(valor.bodegaId),
          cantidadInicial: Number(valor.cantidadInicial),
        });

    solicitud.subscribe({
      next: (respuesta) => {
        if (respuesta?.error) {
          this.mensajeError = respuesta.error;
          this.procesando = false;
          this.changeDetectorRef.markForCheck();
          return;
        }

        this.mensajeExito = this.editando
          ? 'Producto actualizado correctamente.'
          : 'Producto e inventario inicial registrados correctamente.';
        this.prepararCreacion();
        this.procesando = false;
        this.changeDetectorRef.markForCheck();
        this.cargarProductosEInventario();
      },
      error: (error) => {
        this.mensajeError = error?.error?.error ?? 'No fue posible guardar el producto.';
        this.procesando = false;
        this.changeDetectorRef.markForCheck();
      },
    });
  }

  editar(producto: IProducto): void {
    if (this.procesando) {
      return;
    }

    this.mensajeError = '';
    this.mensajeExito = '';
    this.editando = true;
    this.formulario.patchValue({
      productoId: producto.productoId,
      nombre: producto.nombre,
      descripcion: producto.descripcion ?? '',
      precio: producto.precio,
      costoCompra: producto.costoCompra ?? null,
      subcategoriaId: producto.subcategoriaId,
      marcaId: producto.marcaId,
      proveedorId: producto.proveedorId,
    });
    this.formulario.controls.bodegaId.disable({ emitEvent: false });
    this.formulario.controls.cantidadInicial.disable({ emitEvent: false });
    this.formulario.markAsPristine();
  }

  cancelarEdicion(): void {
    if (!this.procesando) {
      this.prepararCreacion();
    }
  }

  cambiarEstado(producto: IProducto): void {
    if (this.procesando) {
      return;
    }

    this.mensajeError = '';
    this.mensajeExito = '';
    this.procesando = true;

    this.productoService.cambiarEstado({
      productoId: producto.productoId,
      activo: !producto.activo,
    }).subscribe({
      next: (respuesta) => {
        if (respuesta?.error) {
          this.mensajeError = respuesta.error;
        } else {
          this.mensajeExito = producto.activo
            ? 'Producto desactivado correctamente.'
            : 'Producto activado correctamente.';
          this.cargarProductosEInventario();
        }
        this.procesando = false;
        this.changeDetectorRef.markForCheck();
      },
      error: (error) => {
        this.mensajeError = error?.error?.error ?? 'No fue posible actualizar el estado del producto.';
        this.procesando = false;
        this.changeDetectorRef.markForCheck();
      },
    });
  }

  seleccionarProducto(producto: IProducto): void {
    this.productoSeleccionado = producto;
    this.actualizarInventariosSeleccionados();
  }

  actualizarCantidadAjuste(inventarioId: number, valor: string): void {
    this.ajustesInventario[inventarioId] = Number(valor);
  }

  guardarAjuste(inventario: IInventario): void {
    const cantidad = this.ajustesInventario[inventario.inventarioId];
    if (this.procesando) {
      return;
    }

    if (!Number.isInteger(cantidad) || cantidad < 0) {
      this.mensajeError = 'La cantidad de inventario debe ser un número entero igual o mayor que cero.';
      return;
    }

    this.mensajeError = '';
    this.mensajeExito = '';
    this.procesando = true;

    this.inventarioService.modificar({
      inventarioId: inventario.inventarioId,
      cantidad,
    }).subscribe({
      next: (respuesta) => {
        if (respuesta?.error) {
          this.mensajeError = respuesta.error;
        } else {
          this.mensajeExito = 'Inventario actualizado correctamente.';
          this.cargarProductosEInventario();
        }
        this.procesando = false;
        this.changeDetectorRef.markForCheck();
      },
      error: (error) => {
        this.mensajeError = error?.error?.error ?? 'No fue posible actualizar el inventario.';
        this.procesando = false;
        this.changeDetectorRef.markForCheck();
      },
    });
  }

  totalInventario(productoId: number): number {
    return this.inventarios
      .filter((inventario) => inventario.productoId === productoId)
      .reduce((total, inventario) => total + inventario.cantidad, 0);
  }

  etiquetaStock(productoId: number): string {
    const total = this.totalInventario(productoId);
    if (total === 0) {
      return 'Sin existencias';
    }

    return total <= 5 ? 'Stock bajo' : 'Stock disponible';
  }

  claseStock(productoId: number): string {
    return this.totalInventario(productoId) <= 5 ? 'text-danger fw-semibold' : 'text-success';
  }

  get productosFiltrados(): IProducto[] {
    const filtros = this.filtros.getRawValue();
    const nombre = this.normalizarTexto(filtros.nombre ?? '');
    const marcaId = Number(filtros.marcaId);
    const categoriaId = Number(filtros.categoriaId);
    const subcategoriaId = Number(filtros.subcategoriaId);

    return this.productos.filter((producto) => {
      const subcategoria = this.subcategorias.find(
        (item) => item.subcategoriaId === producto.subcategoriaId,
      );

      return (!nombre || this.normalizarTexto(producto.nombre).includes(nombre))
        && (!marcaId || producto.marcaId === marcaId)
        && (!categoriaId || subcategoria?.categoriaId === categoriaId)
        && (!subcategoriaId || producto.subcategoriaId === subcategoriaId);
    });
  }

  get subcategoriasFiltro(): ISubcategoria[] {
    const categoriaId = Number(this.filtros.controls.categoriaId.value);
    return categoriaId
      ? this.subcategorias.filter((subcategoria) => subcategoria.categoriaId === categoriaId)
      : this.subcategorias;
  }

  alCambiarCategoria(): void {
    const categoriaId = Number(this.filtros.controls.categoriaId.value);
    const subcategoriaId = Number(this.filtros.controls.subcategoriaId.value);
    const subcategoriaEsValida = this.subcategorias.some(
      (subcategoria) => subcategoria.subcategoriaId === subcategoriaId
        && (!categoriaId || subcategoria.categoriaId === categoriaId),
    );

    if (subcategoriaId && !subcategoriaEsValida) {
      this.filtros.patchValue({ subcategoriaId: 0 });
    }
  }

  restablecerFiltros(): void {
    this.filtros.reset({
      nombre: '',
      marcaId: 0,
      categoriaId: 0,
      subcategoriaId: 0,
    });
  }

  nombreBodega(bodegaId: number): string {
    return this.bodegas.find((bodega) => bodega.bodegaId === bodegaId)?.nombre ?? 'Bodega no disponible';
  }

  campoInvalido(nombreCampo: string): boolean {
    const control = this.formulario.get(nombreCampo);
    return !!control && control.invalid && (control.touched || control.dirty);
  }

  private cargarProductosEInventario(): void {
    const productoSeleccionadoId = this.productoSeleccionado?.productoId;
    forkJoin({
      productos: this.productoService.listarAdministracion(),
      inventarios: this.inventarioService.listar(),
    }).subscribe({
      next: (respuestas) => {
        if (respuestas.productos?.error || respuestas.inventarios?.error) {
          this.mensajeError = 'No fue posible actualizar la información de productos.';
          this.changeDetectorRef.markForCheck();
          return;
        }

        this.productos = respuestas.productos.data ?? [];
        this.inventarios = respuestas.inventarios.data ?? [];
        this.productoSeleccionado = productoSeleccionadoId
          ? this.productos.find((producto) => producto.productoId === productoSeleccionadoId) ?? null
          : null;
        this.actualizarInventariosSeleccionados();
        this.changeDetectorRef.markForCheck();
      },
      error: () => {
        this.mensajeError = 'No fue posible actualizar la información de productos.';
        this.changeDetectorRef.markForCheck();
      },
    });
  }

  private actualizarInventariosSeleccionados(): void {
    this.inventariosProductoSeleccionado = this.productoSeleccionado
      ? this.inventarios.filter((inventario) => inventario.productoId === this.productoSeleccionado?.productoId)
      : [];
    this.ajustesInventario = Object.fromEntries(
      this.inventariosProductoSeleccionado.map((inventario) => [inventario.inventarioId, inventario.cantidad]),
    );
  }

  private prepararCreacion(): void {
    this.editando = false;
    this.formulario.controls.bodegaId.enable({ emitEvent: false });
    this.formulario.controls.cantidadInicial.enable({ emitEvent: false });
    this.formulario.reset({
      productoId: 0,
      nombre: '',
      descripcion: '',
      precio: 0,
      costoCompra: null,
      subcategoriaId: 0,
      marcaId: 0,
      proveedorId: 0,
      bodegaId: 0,
      cantidadInicial: 0,
    });
  }

  private normalizarTexto(valor: string): string {
    return valor.trim().toLocaleLowerCase();
  }
}
