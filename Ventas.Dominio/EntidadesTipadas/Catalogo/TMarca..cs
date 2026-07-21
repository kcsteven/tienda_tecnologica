using System;

namespace Ventas.Dominio.EntidadesTipadas
{
    public class TMarca
    {
        public int MarcaId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? PaisOrigen { get; set; }
        public bool Activo { get; set; }
        public DateTime CreadoEn { get; set; }
        public string? CreadoPor { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public string? ActualizadoPor { get; set; }
    }
}