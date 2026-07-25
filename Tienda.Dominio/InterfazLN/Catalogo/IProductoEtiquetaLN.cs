
using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IProductoEtiquetaLN
    {
        Task<Respuesta<TProductoEtiqueta>> InsertarAsync(TProductoEtiqueta datos);
        Task<Respuesta<bool>> EliminarAsync(TProductoEtiqueta datos);
        Task<Respuesta<IEnumerable<TProductoEtiqueta>>> ListarPorProductoAsync(int productoId);
    }
}