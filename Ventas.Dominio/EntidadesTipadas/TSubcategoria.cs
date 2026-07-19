using System;

namespace Ventas.Dominio.EntidadesTipadas
{
    public class TSubcategoria
    {
        public int SubcategoriaId { get; set; }
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = null!;
        public bool Activo { get; set; }
        public DateTime CreadoEn { get; set; }
        public string? CreadoPor { get; set; }
        public DateTime? ActualizadoEn { get; set; }
        public string? ActualizadoPor { get; set; }
    }
}