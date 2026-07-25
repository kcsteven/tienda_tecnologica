using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
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