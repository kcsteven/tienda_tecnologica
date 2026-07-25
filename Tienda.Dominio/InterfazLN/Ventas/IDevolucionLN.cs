
using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IDevolucionLN
    {
        Task<Respuesta<TDevolucion>> InsertarAsync(TDevolucion datos);
        Task<Respuesta<TDevolucion>> ModificarAsync(TDevolucion datos);
        Task<Respuesta<IEnumerable<TDevolucion>>> ListarPorPedidoAsync(int pedidoId);
    }
}