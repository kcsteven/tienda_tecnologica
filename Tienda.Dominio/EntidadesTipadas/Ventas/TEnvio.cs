
using System;
namespace Tienda.Dominio.EntidadesTipadas
{
    public class TEnvio
    {
        public int EnvioId { get; set; }
        public int PedidoId { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaEstimadaEntrega { get; set; }
        public DateTime? FechaEntregaReal { get; set; }
        public string? NumeroSeguimiento { get; set; }
        public string? Transportista { get; set; }
    }
}