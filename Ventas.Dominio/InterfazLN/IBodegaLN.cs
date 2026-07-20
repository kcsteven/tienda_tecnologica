using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IBodegaLN
    {
        Task<Respuesta<TBodega>> InsertarAsync(TBodega datos);
        Task<Respuesta<TBodega>> ModificarAsync(TBodega datos);
        Task<Respuesta<bool>> EliminarAsync(TBodega datos);
        Task<Respuesta<IEnumerable<TBodega>>> ListarAsync();
        Task<Respuesta<IEnumerable<TBodega>>> BuscarAsync(TBodega datos);
        Task<Respuesta<TBodega>> ObtenerAsync(TBodega datos);
    }
}