using System;

namespace Ventas.Dominio.EntidadesTipadas
{
    public class TCliente
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public DateTime CreadoEn { get; set; }
        public string? CreadoPor { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public string? ActualizadoPor { get; set; }
        public int TipoCedula { get; set; }
    }
}