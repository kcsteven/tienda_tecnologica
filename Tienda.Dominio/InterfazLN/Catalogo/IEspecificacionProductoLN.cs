using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IEspecificacionProductoLN
    {
        Task<Respuesta<TEspecificacionProducto>> InsertarAsync(TEspecificacionProducto datos);
        Task<Respuesta<bool>> EliminarAsync(TEspecificacionProducto datos);
        Task<Respuesta<IEnumerable<TEspecificacionProducto>>> ListarPorProductoAsync(int productoId);
    }
}