using System;
using System.Collections.Generic;

namespace Tienda.Dominio.Entidades;

public partial class Cliente
{
    public int ClienteId { get; set; }

    public int PersonaId { get; set; }

    public string Cedula { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public virtual Persona Persona { get; set; } = null!;

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();
    public virtual ICollection<ListaDeseos> ListaDeseos { get; set; } = new List<ListaDeseos>();
    public virtual ICollection<DireccionCliente> DireccionClientes { get; set; } = new List<DireccionCliente>();
}
