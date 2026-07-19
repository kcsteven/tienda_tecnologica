export interface ISubcategoria {
  subcategoriaId: number;
  categoriaId: number;
  nombre: string;
  activo: boolean;
  creadoEn: Date;
  creadoPor: string;
  actualizadoEn?: Date;
  actualizadoPor?: string;
}
