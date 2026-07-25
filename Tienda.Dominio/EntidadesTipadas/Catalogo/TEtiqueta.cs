using System;

namespace Tienda.Dominio.EntidadesTipadas
{
    public class TEtiqueta
    {
        public int EtiquetaId { get; set; }
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
        public DateTime CreadoEn { get; set; }
        public string? CreadoPor { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public string? ActualizadoPor { get; set; }
    }
}