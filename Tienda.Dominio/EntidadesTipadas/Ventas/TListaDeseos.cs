
using System;
namespace Tienda.Dominio.EntidadesTipadas
{
    public class TListaDeseos
    {
        public int ListaDeseosId { get; set; }
        public int ClienteId { get; set; }
        public int ProductoId { get; set; }
        public DateTime FechaAgregado { get; set; }
    }
}