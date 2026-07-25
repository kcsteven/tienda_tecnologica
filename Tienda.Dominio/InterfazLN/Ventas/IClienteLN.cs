using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IClienteLN
    {
        Task<Respuesta<TCliente>> InsertarAsync(TCliente datos);
        Task<Respuesta<TCliente>> ModificarAsync(TCliente datos);
        Task<Respuesta<bool>> EliminarAsync(TCliente datos);
        Task<Respuesta<IEnumerable<TCliente>>> ListarAsync();
        Task<Respuesta<IEnumerable<TCliente>>> BuscarAsync(TCliente datos);
        Task<Respuesta<TCliente>> ObtenerAsync(TCliente datos);
    }
}