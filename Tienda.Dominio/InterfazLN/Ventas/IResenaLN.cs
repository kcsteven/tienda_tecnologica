
using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IResenaLN
    {
        Task<Respuesta<TResena>> InsertarAsync(TResena datos);
        Task<Respuesta<bool>> EliminarAsync(TResena datos);
        Task<Respuesta<IEnumerable<TResena>>> ListarPorProductoAsync(int productoId);
    }
}