namespace Tienda.Dominio.EntidadesTipadas
{
    public class TEspecificacionProducto
    {
        public int EspecificacionId { get; set; }
        public int ProductoId { get; set; }
        public string Etiqueta { get; set; } = null!;
        public string Valor { get; set; } = null!;
        public int Orden { get; set; }
    }
}