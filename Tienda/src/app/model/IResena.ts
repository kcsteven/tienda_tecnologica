export interface IResena {
  resenaId: number;
  clienteId: number;
  productoId: number;
  calificacion: number;
  comentario?: string;
  fecha: Date;
}
