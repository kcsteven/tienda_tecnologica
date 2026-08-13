namespace Tienda.Dominio.EntidadesTipadas
{
    public class TDisponibilidadBodega
    {
        public string NombreBodega { get; set; } = string.Empty;
        public string? Ubicacion { get; set; }
        public int Cantidad { get; set; }
    }
}
