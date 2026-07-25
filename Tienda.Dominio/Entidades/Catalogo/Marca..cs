using System;
using System.Collections.Generic;

namespace Tienda.Dominio.Entidades;

public partial class Marca
{
    public int MarcaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? PaisOrigen { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public string? CreadoPor { get; set; }
    public DateTime? ActualizadoEn { get; set; }
    public string? ActualizadoPor { get; set; }
    public byte[] RowVer { get; set; } = null!;

    public virtual ICollection<Producto> Productos { get; set; } = new List<Producto>();
}