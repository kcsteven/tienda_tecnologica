export interface ICategoria {
  categoriaId: number;
  nombre: string;
  descripcion?: string;
  activo: boolean;
  creadoEn: Date;
  creadoPor: string;
  actualizadoEn?: Date;
  actualizadoPor?: string;
}
