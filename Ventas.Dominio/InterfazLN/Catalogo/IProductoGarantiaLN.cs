
using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IProductoGarantiaLN
    {
        Task<Respuesta<TProductoGarantia>> InsertarAsync(TProductoGarantia datos);
        Task<Respuesta<bool>> EliminarAsync(TProductoGarantia datos);
        Task<Respuesta<IEnumerable<TProductoGarantia>>> ListarPorProductoAsync(int productoId);
    }
}