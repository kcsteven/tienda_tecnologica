using System;

namespace Tienda.Dominio.Entidades;

public partial class Garantia
{
    public int GarantiaId { get; set; }
    public string? Nombre { get; set; }
    public int Meses { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public string? CreadoPor { get; set; }
    public DateTime? ActualizadoEn { get; set; }
    public string? ActualizadoPor { get; set; }
    public byte[] RowVer { get; set; } = null!;

    public virtual ICollection<ProductoGarantia> ProductoGarantias { get; set; } = new List<ProductoGarantia>();
}