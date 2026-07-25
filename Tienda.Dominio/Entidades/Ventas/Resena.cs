
using System;

namespace Tienda.Dominio.Entidades;

public partial class Resena
{
    public int ResenaId { get; set; }
    public int ClienteId { get; set; }
    public int ProductoId { get; set; }
    public int Calificacion { get; set; }
    public string? Comentario { get; set; }
    public DateTime Fecha { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;
    public virtual Producto Producto { get; set; } = null!;
}