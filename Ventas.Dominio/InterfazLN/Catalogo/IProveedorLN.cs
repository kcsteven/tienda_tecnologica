using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IProveedorLN
    {
        Task<Respuesta<TProveedor>> InsertarAsync(TProveedor datos);
        Task<Respuesta<TProveedor>> ModificarAsync(TProveedor datos);
        Task<Respuesta<bool>> EliminarAsync(TProveedor datos);
        Task<Respuesta<IEnumerable<TProveedor>>> ListarAsync();
        Task<Respuesta<IEnumerable<TProveedor>>> BuscarAsync(TProveedor datos);
        Task<Respuesta<TProveedor>> ObtenerAsync(TProveedor datos);
    }
}