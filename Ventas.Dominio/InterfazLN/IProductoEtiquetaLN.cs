
using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IProductoEtiquetaLN
    {
        Task<Respuesta<TProductoEtiqueta>> InsertarAsync(TProductoEtiqueta datos);
        Task<Respuesta<bool>> EliminarAsync(TProductoEtiqueta datos);
        Task<Respuesta<IEnumerable<TProductoEtiqueta>>> ListarPorProductoAsync(int productoId);
    }
}