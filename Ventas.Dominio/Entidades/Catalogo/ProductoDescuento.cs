namespace Ventas.Dominio.Entidades;

public partial class ProductoDescuento
{
    public int ProductoDescuentoId { get; set; }
    public int ProductoId { get; set; }
    public int DescuentoId { get; set; }

    public virtual Producto Producto { get; set; } = null!;
    public virtual Descuento Descuento { get; set; } = null!;
}