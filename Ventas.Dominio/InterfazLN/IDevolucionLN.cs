
using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IDevolucionLN
    {
        Task<Respuesta<TDevolucion>> InsertarAsync(TDevolucion datos);
        Task<Respuesta<TDevolucion>> ModificarAsync(TDevolucion datos);
        Task<Respuesta<IEnumerable<TDevolucion>>> ListarPorPedidoAsync(int pedidoId);
    }
}