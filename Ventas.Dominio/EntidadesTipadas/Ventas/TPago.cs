
using System;
namespace Ventas.Dominio.EntidadesTipadas
{
    public class TPago
    {
        public int PagoId { get; set; }
        public int PedidoId { get; set; }
        public string MetodoPago { get; set; } = null!;
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string? Referencia { get; set; }
    }
}