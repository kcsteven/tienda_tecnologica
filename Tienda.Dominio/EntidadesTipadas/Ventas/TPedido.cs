using System;
using System.Collections.Generic;
using System.Text;

namespace Tienda.Dominio.EntidadesTipadas
{
    public class TPedido
    {
        public int PedidoId { get; set; }

        public string NombrePedido { get; set; } = null!;

        public int ClienteId { get; set; }

        public DateTime FechaPedido { get; set; }

        public string Moneda { get; set; } = null!;

        public decimal? Total { get; set; }

        public DateTime CreadoEn { get; set; }

        public string? CreadoPor { get; set; }

        public DateTime? ActualizadoEn { get; set; }

        public string? ActualizadoPor { get; set; }
    }
}
