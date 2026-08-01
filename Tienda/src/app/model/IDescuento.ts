export interface IDescuento {
  descuentoId: number;
  nombre: string;
  porcentaje: number;
  fechaInicio?: Date;
  fechaFin?: Date;
}
