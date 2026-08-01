using System;

namespace Tienda.Dominio.EntidadesTipadas
{
    public class TDescuento
    {
        public int DescuentoId { get; set; }
        public string Nombre { get; set; } = null!;
        public decimal Porcentaje { get; set; }
        public DateOnly? FechaInicio { get; set; }
        public DateOnly? FechaFin { get; set; }
        public bool Activo { get; set; }
        public DateTime CreadoEn { get; set; }
        public string? CreadoPor { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public string? ActualizadoPor { get; set; }
    }
}