using System;
using System.Collections.Generic;
using System.Text;

namespace Tienda.Dominio.EntidadesTipadas
{
    public class TPedidoCrearDTO
    {
        public TPedido Pedido { get; set; } = null!;
        public string CorreoCliente { get; set; } = null!;
        public string NombreCliente { get; set; } = null!;
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
    }
}