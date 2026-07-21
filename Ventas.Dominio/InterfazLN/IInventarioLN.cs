
using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IInventarioLN
    {
        Task<Respuesta<TInventario>> InsertarAsync(TInventario datos);
        Task<Respuesta<TInventario>> ModificarAsync(TInventario datos);
        Task<Respuesta<bool>> EliminarAsync(TInventario datos);
        Task<Respuesta<IEnumerable<TInventario>>> ListarAsync();
        Task<Respuesta<IEnumerable<TInventario>>> ListarPorProductoAsync(int productoId);
        Task<Respuesta<TInventario>> ObtenerAsync(TInventario datos);
    }
}