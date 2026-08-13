
using System;
namespace Tienda.Dominio.EntidadesTipadas
{
    public class TPago
    {
        public int PagoId { get; set; }
        public int PedidoId { get; set; }
        public int MetodoPagoId { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string? Referencia { get; set; }

        public string? MetodoPagoNombre { get; set; }
    }
}