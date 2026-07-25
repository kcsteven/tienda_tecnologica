
using System;
namespace Tienda.Dominio.EntidadesTipadas
{
    public class TResena
    {
        public int ResenaId { get; set; }
        public int ClienteId { get; set; }
        public int ProductoId { get; set; }
        public int Calificacion { get; set; }
        public string? Comentario { get; set; }
        public DateTime Fecha { get; set; }
    }
}