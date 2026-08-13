namespace Tienda.Dominio.EntidadesTipadas
{
    // Representa una línea de producto para armar la factura por correo
    public class TItemFactura
    {
        public string Nombre { get; set; } = null!;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public string? ImagenUrl { get; set; }
    }
}