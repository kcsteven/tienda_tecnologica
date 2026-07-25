
using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IProductoGarantiaLN
    {
        Task<Respuesta<TProductoGarantia>> InsertarAsync(TProductoGarantia datos);
        Task<Respuesta<bool>> EliminarAsync(TProductoGarantia datos);
        Task<Respuesta<IEnumerable<TProductoGarantia>>> ListarPorProductoAsync(int productoId);
    }
}