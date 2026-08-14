using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Tienda.Dominio.EntidadesTipadas
{
    public class TDetallePedidoCrear
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "El producto seleccionado no es valido")]
        public int ProductoId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "La cantidad debe ser mayor que cero")]
        public int Cantidad { get; set; }
    }

    public class TPedidoCrear
    {
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "El cliente no es valido")]
        public int ClienteId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "La dirección no es valida")]
        public int? DireccionId { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "El metodo de pago no es valido")]
        public int MetodoPagoId { get; set; }

        [MinLength(
            1,
            ErrorMessage = "Debe agregar al menos un producto")]
        public List<TDetallePedidoCrear> Detalles { get; set; } = new List<TDetallePedidoCrear>();
    }
}