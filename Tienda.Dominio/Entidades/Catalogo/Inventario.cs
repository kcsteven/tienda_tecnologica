namespace Tienda.Dominio.Entidades;

public partial class Inventario
{
    public int InventarioId { get; set; }
    public int ProductoId { get; set; }
    public int BodegaId { get; set; }
    public int Cantidad { get; set; }

    public virtual Producto Producto { get; set; } = null!;
    public virtual Bodega Bodega { get; set; } = null!;
}