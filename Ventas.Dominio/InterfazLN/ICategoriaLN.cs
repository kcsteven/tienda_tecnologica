using System;
using System.Collections.Generic;
using System.Text;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Utilidades;

namespace Ventas.Dominio.InterfazLN
{
    public interface ICategoriaLN
    {
        Task<Respuesta<TCategorium>> InsertarAsync(TCategorium datos);
        Task<Respuesta<TCategorium>> ModificarAsync(TCategorium datos);

        Task<Respuesta<bool>> EliminarAsync(TCategorium datos);

        Task<Respuesta<IEnumerable<TCategorium>>> ListarAsync();

        Task<Respuesta<IEnumerable<TCategorium>>> BuscarAsync(TCategorium datos);

        Task<Respuesta<TCategorium>> ObtenerAsync(TCategorium datos);
    }
}
