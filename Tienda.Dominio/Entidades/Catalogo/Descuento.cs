using System;
using System.Collections.Generic;

namespace Tienda.Dominio.Entidades;

public partial class Descuento
{
    public int DescuentoId { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Porcentaje { get; set; }
    public DateOnly? FechaInicio { get; set; }
    public DateOnly? FechaFin { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public string? CreadoPor { get; set; }
    public DateTime? ActualizadoEn { get; set; }
    public string? ActualizadoPor { get; set; }
    public byte[] RowVer { get; set; } = null!;

    public virtual ICollection<ProductoDescuento> ProductoDescuentos { get; set; } = new List<ProductoDescuento>();
}