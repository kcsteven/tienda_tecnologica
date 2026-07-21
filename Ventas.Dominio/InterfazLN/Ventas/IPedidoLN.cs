using System;
using System.Collections.Generic;
using System.Text;
using Ventas.Dominio.Entidades;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IPedidoLN
    {
        Task<Respuesta<TPedido>> InsertarAsync(TPedido datos);
        Task<Respuesta<TPedido>> ModificarAsync(TPedido datos);
        Task<Respuesta<bool>> EliminarAsync(TPedido datos);
        Task<Respuesta<IEnumerable<TPedido>>> BuscarAsync(TPedido datos);
        Task<Respuesta<TPedido>> ObtenerAsync(TPedido datos);
        Task<Respuesta<IEnumerable<TPedido>>> ListarAsync();

    }
}
