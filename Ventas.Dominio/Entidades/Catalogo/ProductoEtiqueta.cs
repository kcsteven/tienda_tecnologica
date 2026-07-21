namespace Ventas.Dominio.Entidades;

public partial class ProductoEtiqueta
{
    public int ProductoEtiquetaId { get; set; }
    public int ProductoId { get; set; }
    public int EtiquetaId { get; set; }

    public virtual Producto Producto { get; set; } = null!;
    public virtual Etiqueta Etiqueta { get; set; } = null!;
}