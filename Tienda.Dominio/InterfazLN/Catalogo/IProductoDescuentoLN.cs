
using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IProductoDescuentoLN
    {
        Task<Respuesta<TProductoDescuento>> InsertarAsync(TProductoDescuento datos);
        Task<Respuesta<bool>> EliminarAsync(TProductoDescuento datos);
        Task<Respuesta<IEnumerable<TProductoDescuento>>> ListarPorProductoAsync(int productoId);
    }
}