using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfacesAD
{
    public interface IRepositorioAD<TEntity> where TEntity : class
    {
        Task<Respuesta<TEntity>> InsertarAsync(TEntity objEntidad);

        Task<Respuesta<TEntity>> ModificarAsync(TEntity objEntidad);

        Task<Respuesta<bool>> EliminarAsync(TEntity objEntidad);

        Task<Respuesta<IEnumerable<TEntity>>> ListarAsync(List<string>? objIncludes = null);

        Task<Respuesta<IEnumerable<TEntity>>> BuscarAsync(Expression<Func<TEntity, bool>> objPredicado, List<string>? objIncludes = null);

        Task<Respuesta<TEntity>> ObtenerEntidadAsync(Expression<Func<TEntity, bool>> objPredicado, List<string>? objIncludes = null);
    }
}
