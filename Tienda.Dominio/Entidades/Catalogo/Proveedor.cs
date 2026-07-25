using System;
using System.Collections.Generic;

namespace Tienda.Dominio.Entidades;

public partial class Proveedor
{
    public int ProveedorId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public string? CreadoPor { get; set; }
    public DateTime? ActualizadoEn { get; set; }
    public string? ActualizadoPor { get; set; }
    public byte[] RowVer { get; set; } = null!;

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}