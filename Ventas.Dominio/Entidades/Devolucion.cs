
using System;

namespace Ventas.Dominio.Entidades;

public partial class Devolucion
{
    public int DevolucionId { get; set; }
    public int PedidoId { get; set; }
    public int ProductoId { get; set; }
    public string? Motivo { get; set; }
    public DateTime Fecha { get; set; }
    public string? EstadoDevolucion { get; set; }

    public virtual Pedido Pedido { get; set; } = null!;
    public virtual Producto Producto { get; set; } = null!;
}