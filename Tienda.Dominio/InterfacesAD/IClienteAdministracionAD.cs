using System;
using System.Collections.Generic;
using System.Text;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.EntidadesTipadas.Usuarios;

//Define las consultas y el cambio logico del estado del cliente
namespace Tienda.Dominio.InterfacesAD
{
    public interface IClienteAdministracionAD
    {
        //Obtiene los clientes activos/inactivos sin carga las contraseñas
        Task<IReadOnlyList<TClienteAdministracion>> ListarAsync();

        //Busca clientes validos relacionados al rol Cliente
        Task<TClienteAdministracion?> ObtenerAsync(int clienteId);

        //Modifica el estado de usuarios activos
        Task<int> CambiarEstadoAsync(int clienteId, bool activo);
    }
}
