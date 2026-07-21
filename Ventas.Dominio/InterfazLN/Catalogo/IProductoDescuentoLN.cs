
using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IProductoDescuentoLN
    {
        Task<Respuesta<TProductoDescuento>> InsertarAsync(TProductoDescuento datos);
        Task<Respuesta<bool>> EliminarAsync(TProductoDescuento datos);
        Task<Respuesta<IEnumerable<TProductoDescuento>>> ListarPorProductoAsync(int productoId);
    }
}