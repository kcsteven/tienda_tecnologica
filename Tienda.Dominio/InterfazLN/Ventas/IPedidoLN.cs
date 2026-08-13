using System;
using System.Collections.Generic;
using System.Text;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IPedidoLN
    {
        Task<Respuesta<TPedido>> InsertarAsync(TPedido datos);
        Task<Respuesta<TPedido>> ModificarAsync(TPedido datos);
        Task<Respuesta<bool>> EliminarAsync(TPedido datos);
        Task<Respuesta<IEnumerable<TPedido>>> BuscarAsync(TPedido datos);
        Task<Respuesta<TPedido>> ObtenerAsync(TPedido datos);
        Task<Respuesta<IEnumerable<TPedido>>> ListarAsync();
        Task<Respuesta<TPedido>> CrearCompraAsync(TPedidoCrear datos);

    }
}
