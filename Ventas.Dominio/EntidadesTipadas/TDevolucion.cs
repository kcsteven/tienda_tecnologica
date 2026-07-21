
using System;
namespace Ventas.Dominio.EntidadesTipadas
{
    public class TDevolucion
    {
        public int DevolucionId { get; set; }
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public string? Motivo { get; set; }
        public DateTime Fecha { get; set; }
        public string? EstadoDevolucion { get; set; }
    }
}