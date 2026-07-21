using System;

namespace Ventas.Dominio.Entidades;

public partial class Subcategoria
{
    public int SubcategoriaId { get; set; }
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public string? CreadoPor { get; set; }
    public DateTime? ActualizadoEn { get; set; }
    public string? ActualizadoPor { get; set; }
    public byte[] RowVer { get; set; } = null!;

    public virtual Categoria Categoria { get; set; } = null!;
    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}