using System;
using System.Collections.Generic;
using System.Text;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IProductoLN
    {

        Task<Respuesta<TProducto>> InsertarAsync(TProducto datos);
        Task<Respuesta<TProducto>> ModificarAsync(TProducto datos);
        Task<Respuesta<bool>> EliminarAsync(TProducto datos);
        Task<Respuesta<IEnumerable<TProducto>>> BuscarAsync(TProducto datos);
        Task<Respuesta<TProducto>> ObtenerAsync(TProducto datos);
        Task<Respuesta<IEnumerable<TProducto>>> ListarAsync();


    }
}
