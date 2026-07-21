namespace Ventas.Dominio.Entidades;

public partial class ImagenProducto
{
    public int ImagenId { get; set; }
    public int ProductoId { get; set; }
    public string? RutaImagen { get; set; }

    public virtual Producto Producto { get; set; } = null!;
}