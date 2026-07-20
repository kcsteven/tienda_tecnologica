using System;

namespace Ventas.Dominio.Entidades;

public partial class Descuento
{
    public int DescuentoId { get; set; }
    public string Nombre { get; set; } = null!;
    public decimal Porcentaje { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public string? CreadoPor { get; set; }
    public DateTime? ActualizadoEn { get; set; }
    public string? ActualizadoPor { get; set; }
    public byte[] RowVer { get; set; } = null!;
}