
using System;

namespace Ventas.Dominio.Entidades;

public partial class Envio
{
    public int EnvioId { get; set; }
    public int PedidoId { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public DateTime? FechaEstimadaEntrega { get; set; }
    public DateTime? FechaEntregaReal { get; set; }
    public string? NumeroSeguimiento { get; set; }
    public string? Transportista { get; set; }

    public virtual Pedido Pedido { get; set; } = null!;
}