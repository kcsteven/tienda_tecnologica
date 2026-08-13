namespace Tienda.Dominio.EntidadesTipadas
{
    public class TCrearProductoConInventario
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public decimal? CostoCompra { get; set; }
        public int SubcategoriaId { get; set; }
        public int MarcaId { get; set; }
        public int ProveedorId { get; set; }
        public int BodegaId { get; set; }
        public int CantidadInicial { get; set; }
    }
}
