namespace Tienda.Dominio.Entidades;

public partial class EspecificacionProducto
{
    public int EspecificacionId { get; set; }
    public int ProductoId { get; set; }
    public string Etiqueta { get; set; } = null!;
    public string Valor { get; set; } = null!;
    public int Orden { get; set; }

    public virtual Producto Producto { get; set; } = null!;
}