
using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IImagenProductoLN
    {
        Task<Respuesta<TImagenProducto>> InsertarAsync(TImagenProducto datos);
        Task<Respuesta<bool>> EliminarAsync(TImagenProducto datos);
        Task<Respuesta<IEnumerable<TImagenProducto>>> ListarPorProductoAsync(int productoId);
    }
}