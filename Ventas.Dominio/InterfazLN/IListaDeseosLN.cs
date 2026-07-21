
using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IListaDeseosLN
    {
        Task<Respuesta<TListaDeseos>> InsertarAsync(TListaDeseos datos);
        Task<Respuesta<bool>> EliminarAsync(TListaDeseos datos);
        Task<Respuesta<IEnumerable<TListaDeseos>>> ListarPorClienteAsync(int clienteId);
    }
}