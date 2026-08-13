using System.Collections.Generic;

namespace Tienda.Dominio.Entidades;

public partial class EstadoPedido
{
    public int EstadoPedidoId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}