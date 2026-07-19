using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface ICategoriaLN
    {
        Task<Respuesta<TCategoria>> InsertarAsync(TCategoria datos);
        Task<Respuesta<TCategoria>> ModificarAsync(TCategoria datos);
        Task<Respuesta<bool>> EliminarAsync(TCategoria datos);
        Task<Respuesta<IEnumerable<TCategoria>>> ListarAsync();
        Task<Respuesta<IEnumerable<TCategoria>>> BuscarAsync(TCategoria datos);
        Task<Respuesta<TCategoria>> ObtenerAsync(TCategoria datos);
    }
}