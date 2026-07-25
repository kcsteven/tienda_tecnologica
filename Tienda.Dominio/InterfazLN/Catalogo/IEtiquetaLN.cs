using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IEtiquetaLN
    {
        Task<Respuesta<TEtiqueta>> InsertarAsync(TEtiqueta datos);
        Task<Respuesta<TEtiqueta>> ModificarAsync(TEtiqueta datos);
        Task<Respuesta<bool>> EliminarAsync(TEtiqueta datos);
        Task<Respuesta<IEnumerable<TEtiqueta>>> ListarAsync();
        Task<Respuesta<IEnumerable<TEtiqueta>>> BuscarAsync(TEtiqueta datos);
        Task<Respuesta<TEtiqueta>> ObtenerAsync(TEtiqueta datos);
    }
}