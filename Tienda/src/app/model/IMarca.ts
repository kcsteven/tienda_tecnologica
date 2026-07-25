export interface IMarca {
  marcaId: number;
  nombre: string;
  paisOrigen?: string;
  activo: boolean;
  creadoEn: Date;
  creadoPor: string;
  actualizadoEn?: Date;
  actualizadoPor?: string;
}
