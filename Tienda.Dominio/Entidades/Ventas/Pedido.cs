using System;
using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;

namespace Tienda.Dominio.Entidades;

public partial class Pedido
{
    public int PedidoId { get; set; }

    public int ClienteId { get; set; }

    public int? EmpleadoId { get; set; }

    public int EstadoPedidoId { get; set; }

    public int? DireccionId { get; set; }

    public DateTime FechaPedido { get; set; }

    public decimal Total { get; set; }

    // Propiedades de navegación
    public virtual Cliente Cliente { get; set; } = null!;

    public virtual EstadoPedido EstadoPedido { get; set; } = null!;

    public virtual DireccionCliente? Direccion { get; set; }

    public virtual ICollection<DetallesPedido> DetallesPedido { get; set; }
        = new List<DetallesPedido>();

    public virtual ICollection<Pago> Pagos { get; set; }
        = new List<Pago>();
}