using System.Collections.Generic;

namespace Tienda.Dominio.EntidadesTipadas
{
    public class TDetallePedidoCrear
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }

    public class TPedidoCrear
    {
        public int ClienteId { get; set; }
        public int? DireccionId { get; set; }
        public int MetodoPagoId { get; set; }
        public List<TDetallePedidoCrear> Detalles { get; set; } = new List<TDetallePedidoCrear>();
    }
}