
using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IResenaLN
    {
        Task<Respuesta<TResena>> InsertarAsync(TResena datos);
        Task<Respuesta<bool>> EliminarAsync(TResena datos);
        Task<Respuesta<IEnumerable<TResena>>> ListarPorProductoAsync(int productoId);
    }
}