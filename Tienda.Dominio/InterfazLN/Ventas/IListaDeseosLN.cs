
using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IListaDeseosLN
    {
        Task<Respuesta<TListaDeseos>> InsertarAsync(TListaDeseos datos);
        Task<Respuesta<bool>> EliminarAsync(TListaDeseos datos);
        Task<Respuesta<IEnumerable<TListaDeseos>>> ListarPorClienteAsync(int clienteId);
    }
}