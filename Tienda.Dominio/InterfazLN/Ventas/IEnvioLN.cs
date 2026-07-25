
using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IEnvioLN
    {
        Task<Respuesta<TEnvio>> InsertarAsync(TEnvio datos);
        Task<Respuesta<TEnvio>> ModificarAsync(TEnvio datos);
        Task<Respuesta<TEnvio>> ObtenerPorPedidoAsync(int pedidoId);
    }
}