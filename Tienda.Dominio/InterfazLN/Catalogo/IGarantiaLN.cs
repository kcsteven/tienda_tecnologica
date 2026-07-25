using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IGarantiaLN
    {
        Task<Respuesta<TGarantia>> InsertarAsync(TGarantia datos);
        Task<Respuesta<TGarantia>> ModificarAsync(TGarantia datos);
        Task<Respuesta<bool>> EliminarAsync(TGarantia datos);
        Task<Respuesta<IEnumerable<TGarantia>>> ListarAsync();
        Task<Respuesta<IEnumerable<TGarantia>>> BuscarAsync(TGarantia datos);
        Task<Respuesta<TGarantia>> ObtenerAsync(TGarantia datos);
    }
}