using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

//Datos minimos para activar/desactivar un cliente
namespace Tienda.Dominio.EntidadesTipadas.Usuarios
{
    public class TCambiarEstadoCliente
    {
        [Range(1, int.MaxValue, ErrorMessage = "El cliente seleccionado no es valido")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Debe indicar el estado del cliente")]
        public bool? Activo { get; set; }

    }
}
