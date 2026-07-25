using System;
using System.Collections.Generic;

namespace Tienda.Dominio.Entidades;

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
    public virtual ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();
    public virtual ICollection<ProductoDescuento> ProductoDescuentos { get; set; } = new List<ProductoDescuento>();
    public virtual ICollection<ImagenProducto> ImagenProductos { get; set; } = new List<ImagenProducto>();
    public virtual ICollection<ProductoGarantia> ProductoGarantias { get; set; } = new List<ProductoGarantia>();
    public virtual ICollection<ProductoEtiqueta> ProductoEtiquetas { get; set; } = new List<ProductoEtiqueta>();
    public virtual ICollection<Resena> Resenas { get; set; } = new List<Resena>();
    public virtual ICollection<ListaDeseos> ListaDeseos { get; set; } = new List<ListaDeseos>();
    public virtual ICollection<Devolucion> Devoluciones { get; set; } = new List<Devolucion>();

}