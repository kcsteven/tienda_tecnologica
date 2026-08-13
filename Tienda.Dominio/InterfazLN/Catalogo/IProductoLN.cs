using System;
using System.Collections.Generic;
using System.Text;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IProductoLN
    {

        Task<Respuesta<TProducto>> InsertarAsync(TCrearProductoConInventario datos, int usuarioId);
        Task<Respuesta<TProducto>> ModificarAsync(TActualizarProducto datos, int usuarioId);
        Task<Respuesta<TProducto>> CambiarEstadoAsync(TCambiarEstadoProducto datos, int usuarioId);
        Task<Respuesta<bool>> EliminarAsync(TProducto datos);
        Task<Respuesta<IEnumerable<TProducto>>> BuscarAsync(TProducto datos);
        Task<Respuesta<TProducto>> ObtenerAsync(TProducto datos);
        Task<Respuesta<IEnumerable<TProducto>>> ListarAsync();
        Task<Respuesta<IEnumerable<TProducto>>> ListarAdministracionAsync();


    }
}
