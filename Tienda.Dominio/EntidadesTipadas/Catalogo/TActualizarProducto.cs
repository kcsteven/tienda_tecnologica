namespace Tienda.Dominio.EntidadesTipadas
{
    public class TActualizarProducto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public decimal? CostoCompra { get; set; }
        public int SubcategoriaId { get; set; }
        public int MarcaId { get; set; }
        public int ProveedorId { get; set; }
    }
}
