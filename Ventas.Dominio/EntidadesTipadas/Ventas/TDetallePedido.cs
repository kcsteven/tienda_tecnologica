using System;
using System.Collections.Generic;
using System.Text;

namespace Ventas.Dominio.EntidadesTipadas
{
    public class TDetallePedido
    {
        public int DetalleId { get; set; }

        public int PedidoId { get; set; }

        public string ProductoId { get; set; } = null!;

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Descuento { get; set; }


    }
}
