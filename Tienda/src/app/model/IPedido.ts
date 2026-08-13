export interface IPedido {
  pedidoId: number;
  clienteId: number;
  estadoPedidoId: number;
  direccionId?: number | null;
  fechaPedido: string | Date;
  total: number;
}

export interface IDetallePedidoCrear {
  productoId: number;
  cantidad: number;
}

export interface IPedidoCrear {
  clienteId: number;
  direccionId?: number | null;
  metodoPagoId: number;
  detalles: IDetallePedidoCrear[];
}
