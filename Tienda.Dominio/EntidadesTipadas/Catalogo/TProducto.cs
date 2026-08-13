using System;

namespace Tienda.Dominio.EntidadesTipadas
{
    public class TProducto
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
    }
}
