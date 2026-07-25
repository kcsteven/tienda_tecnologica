
namespace Tienda.Dominio.EntidadesTipadas
{
    public class TInventario
    {
        public int InventarioId { get; set; }
        public int ProductoId { get; set; }
        public int BodegaId { get; set; }
        public int Cantidad { get; set; }
    }
}