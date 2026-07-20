using System;
using System.Collections.Generic;

namespace Ventas.Dominio.Entidades;

public partial class Producto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public decimal? CostoCompra { get; set; }
    public int SubcategoriaId { get; set; }
    public int MarcaId { get; set; }
    public int ProveedorId { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public string? CreadoPor { get; set; }
    public DateTime? ActualizadoEn { get; set; }
    public string? ActualizadoPor { get; set; }
    public byte[] RowVer { get; set; } = null!;

    public virtual Subcategoria Subcategoria { get; set; } = null!;
    public virtual Marca Marca { get; set; } = null!;
    public virtual Proveedor Proveedor { get; set; } = null!;
    public virtual ICollection<DetallesPedido> DetallesPedidos { get; set; } = new List<DetallesPedido>();
}