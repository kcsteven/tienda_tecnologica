using System;
using System.Collections.Generic;
using System.Text;

//Datos seguros que el empleado puede consultar en la administracion de clientes
namespace Tienda.Dominio.EntidadesTipadas.Usuarios
{
    public class TClienteAdministracion
    {
        public int ClienteId { get; set; }

        public int UsuarioId {  get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string NumeroDocumento {  get; set; } = string.Empty ;

        public string? Email { get; set; }

        public string NombreUsuario { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }

        public bool Activo { get; set; }

    }
}
