using System.Collections.Generic;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface IDescuentoLN
    {
        Task<Respuesta<TDescuento>> InsertarAsync(TDescuento datos);
        Task<Respuesta<TDescuento>> ModificarAsync(TDescuento datos);
        Task<Respuesta<bool>> EliminarAsync(TDescuento datos);
        Task<Respuesta<IEnumerable<TDescuento>>> ListarAsync();
        Task<Respuesta<IEnumerable<TDescuento>>> BuscarAsync(TDescuento datos);
        Task<Respuesta<TDescuento>> ObtenerAsync(TDescuento datos);
    }
}