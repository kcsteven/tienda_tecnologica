using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IMarcaLN
    {
        Task<Respuesta<TMarca>> InsertarAsync(TMarca datos);
        Task<Respuesta<TMarca>> ModificarAsync(TMarca datos);
        Task<Respuesta<bool>> EliminarAsync(TMarca datos);
        Task<Respuesta<IEnumerable<TMarca>>> ListarAsync();
        Task<Respuesta<IEnumerable<TMarca>>> BuscarAsync(TMarca datos);
        Task<Respuesta<TMarca>> ObtenerAsync(TMarca datos);
    }
}