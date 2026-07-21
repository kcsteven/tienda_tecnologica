using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
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