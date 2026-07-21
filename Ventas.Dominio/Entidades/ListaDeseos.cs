
using System;

namespace Ventas.Dominio.Entidades;

public partial class ListaDeseos
{
    public int ListaDeseosId { get; set; }
    public int ClienteId { get; set; }
    public int ProductoId { get; set; }
    public DateTime FechaAgregado { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;
    public virtual Producto Producto { get; set; } = null!;
}