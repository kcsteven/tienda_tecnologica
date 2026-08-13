using System;
using System.Collections.Generic;
using System.Text;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IProductoLN
    {

        Task<Respuesta<TProducto>> InsertarAsync(TCrearProductoConInventario datos);
        Task<Respuesta<TProducto>> ModificarAsync(TActualizarProducto datos);
        Task<Respuesta<TProducto>> CambiarEstadoAsync(TCambiarEstadoProducto datos);
        Task<Respuesta<bool>> EliminarAsync(TProducto datos);
        Task<Respuesta<IEnumerable<TProducto>>> BuscarAsync(TProducto datos);
        Task<Respuesta<TProducto>> ObtenerAsync(TProducto datos);
        Task<Respuesta<IEnumerable<TProducto>>> ListarAsync();
        Task<Respuesta<IEnumerable<TProducto>>> ListarAdministracionAsync();


    }
}
