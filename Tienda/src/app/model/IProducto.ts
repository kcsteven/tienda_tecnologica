export interface IProducto {
  productoId: number;
  nombre: string;
  descripcion?: string;
  precio: number;
  costoCompra?: number;
  subcategoriaId: number;
  marcaId: number;
  proveedorId: number;
}
