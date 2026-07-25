using System;
using System.Collections.Generic;

namespace Tienda.Dominio.Entidades;

public partial class Bodega
{
    public int BodegaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Ubicacion { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public string? CreadoPor { get; set; }
    public DateTime? ActualizadoEn { get; set; }
    public string? ActualizadoPor { get; set; }
    public byte[] RowVer { get; set; } = null!;

    public virtual ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();
}