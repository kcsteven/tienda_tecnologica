using System;

namespace Ventas.Dominio.Entidades;

public partial class Etiqueta
{
    public int EtiquetaId { get; set; }
    public string Nombre { get; set; } = null!;
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public string? CreadoPor { get; set; }
    public DateTime? ActualizadoEn { get; set; }
    public string? ActualizadoPor { get; set; }
    public byte[] RowVer { get; set; } = null!;

    public virtual ICollection<ProductoEtiqueta> ProductoEtiquetas { get; set; } = new List<ProductoEtiqueta>();
}