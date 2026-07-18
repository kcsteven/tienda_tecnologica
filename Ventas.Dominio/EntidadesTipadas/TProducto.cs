using System;
using System.Collections.Generic;
using System.Text;

namespace Ventas.Dominio.EntidadesTipadas
{
    public class TProducto
    {
        public int ProductoId { get; set; }

        public string Nombre { get; set; } = null!;

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
