using System;
using System.Collections.Generic;
using System.Text;
using Tienda.Dominio.EntidadesTipadas.Usuarios;
using Tienda.Utilidades;

//Define las operaciones permitidas sobre clientes
namespace Tienda.Dominio.InterfazLN.Usuarios
{
    public interface IClienteLN
    {
        Task<Respuesta<IEnumerable<TClienteAdministracion>>> ListarAdministracionAsync();

        Task<Respuesta<bool>> CambiarEstadoAsync(TCambiarEstadoCliente datos);

    }
}
