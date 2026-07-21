
using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IImagenProductoLN
    {
        Task<Respuesta<TImagenProducto>> InsertarAsync(TImagenProducto datos);
        Task<Respuesta<bool>> EliminarAsync(TImagenProducto datos);
        Task<Respuesta<IEnumerable<TImagenProducto>>> ListarPorProductoAsync(int productoId);
    }
}