using System;
using System.Collections.Generic;

namespace Ventas.Dominio.Entidades;

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
}