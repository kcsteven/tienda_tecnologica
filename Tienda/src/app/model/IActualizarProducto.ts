export interface IActualizarProducto {
  productoId: number;
  nombre: string;
  descripcion: string | null;
  precio: number;
  costoCompra: number | null;
  subcategoriaId: number;
  marcaId: number;
  proveedorId: number;
}
