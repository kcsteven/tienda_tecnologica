
using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IEnvioLN
    {
        Task<Respuesta<TEnvio>> InsertarAsync(TEnvio datos);
        Task<Respuesta<TEnvio>> ModificarAsync(TEnvio datos);
        Task<Respuesta<TEnvio>> ObtenerPorPedidoAsync(int pedidoId);
    }
}