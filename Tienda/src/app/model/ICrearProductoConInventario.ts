export interface ICrearProductoConInventario {
  nombre: string;
  descripcion: string | null;
  precio: number;
  costoCompra: number | null;
  subcategoriaId: number;
  marcaId: number;
  proveedorId: number;
  bodegaId: number;
  cantidadInicial: number;
}
