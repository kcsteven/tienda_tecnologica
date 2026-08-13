export interface IPago {
  pagoId?: number;
  pedidoId: number;
  metodoPagoId: number;
  metodoPagoNombre?: string;
  monto: number;
  fechaPago?: string;
  referencia?: string;
}
