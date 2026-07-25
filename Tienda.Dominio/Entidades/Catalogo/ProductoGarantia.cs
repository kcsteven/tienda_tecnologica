namespace Tienda.Dominio.Entidades;

public partial class ProductoGarantia
{
    public int ProductoGarantiaId { get; set; }
    public int ProductoId { get; set; }
    public int GarantiaId { get; set; }

    public virtual Producto Producto { get; set; } = null!;
    public virtual Garantia Garantia { get; set; } = null!;
}