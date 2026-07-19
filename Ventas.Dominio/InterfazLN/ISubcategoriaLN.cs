using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface ISubcategoriaLN
    {
        Task<Respuesta<TSubcategoria>> InsertarAsync(TSubcategoria datos);
        Task<Respuesta<TSubcategoria>> ModificarAsync(TSubcategoria datos);
        Task<Respuesta<bool>> EliminarAsync(TSubcategoria datos);
        Task<Respuesta<IEnumerable<TSubcategoria>>> ListarAsync();
        Task<Respuesta<IEnumerable<TSubcategoria>>> BuscarAsync(TSubcategoria datos);
        Task<Respuesta<TSubcategoria>> ObtenerAsync(TSubcategoria datos);
        Task<Respuesta<IEnumerable<TSubcategoria>>> ListarPorCategoriaAsync(int categoriaId);
    }
}