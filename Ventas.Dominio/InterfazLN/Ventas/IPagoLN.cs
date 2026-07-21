
using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IPagoLN
    {
        Task<Respuesta<TPago>> InsertarAsync(TPago datos);
        Task<Respuesta<bool>> EliminarAsync(TPago datos);
        Task<Respuesta<IEnumerable<TPago>>> ListarPorPedidoAsync(int pedidoId);
    }
}