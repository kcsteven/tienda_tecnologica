using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Tienda.Dominio.InterfacesAD;
using Tienda.Utilidades;

namespace Tienda.AccesoDatos.Implementaciones
{
    // Repositorio genérico: implementa operaciones CRUD básicas para cualquier entidad
    public class RepositorioAD<TEntity> : IRepositorioAD<TEntity> where TEntity : class
    {

        #region Atributos y Variables


        // Contexto de base de datos usado para las consultas
        protected readonly DbContext _context;


        public RepositorioAD(DbContext context)

        {

            this._context = context;

        }

        // Arma el mensaje de error incluyendo la excepción interna (donde SQL Server
        // reporta el motivo real: violación de FK, columna requerida, tipo de dato, etc.)
        protected static string ObtenerMensajeCompleto(Exception ex)
        {
            return ex.InnerException != null
                ? $"{ex.Message} | Detalle: {ex.InnerException.Message}"
                : ex.Message;
        }



        #endregion#region Métodos Públicos


        // Inserta una nueva entidad en la base de datos
        public async Task<Respuesta<TEntity>> InsertarAsync(TEntity objEntidad)

        {

            Respuesta<TEntity> objRespuesta = new Respuesta<TEntity>();

            try
            {

                await _context.Set<TEntity>().AddAsync(objEntidad);

                await _context.SaveChangesAsync();

                objRespuesta.Data = objEntidad;

            }

            catch (Exception ex)

            {

                // Si ocurre un error, se guarda el mensaje (incluyendo el detalle interno) y no se devuelve dato
                objRespuesta.Error = ObtenerMensajeCompleto(ex);

                objRespuesta.Data = null;

            }

            return objRespuesta;

        }


        // Actualiza una entidad existente en la base de datos
        public async Task<Respuesta<TEntity>> ModificarAsync(TEntity objEntidad)

        {

            Respuesta<TEntity> objRespuesta = new Respuesta<TEntity>();

            try
            {

                _context.Set<TEntity>().Update(objEntidad);

                await _context.SaveChangesAsync();

                objRespuesta.Data = objEntidad;

            }

            catch (Exception ex)

            {

                objRespuesta.Error = ObtenerMensajeCompleto(ex);

                objRespuesta.Data = null;

            }

            return objRespuesta;

        }


        // Elimina una entidad de la base de datos
        public async Task<Respuesta<bool>> EliminarAsync(TEntity objEntidad)

        {

            Respuesta<bool> objRespuesta = new Respuesta<bool>();

            try
            {

                _context.Entry(objEntidad).State = EntityState.Deleted;

                await _context.SaveChangesAsync();

                objRespuesta.Data = true;

            }

            catch (Exception ex)

            {

                objRespuesta.Error = ObtenerMensajeCompleto(ex);

                objRespuesta.Data = false;

            }

            return objRespuesta;

        }


        // Lista todas las entidades, incluyendo relaciones opcionales (objIncludes)
        public async Task<Respuesta<IEnumerable<TEntity>>> ListarAsync(List<string>? objIncludes = null)

        {

            Respuesta<IEnumerable<TEntity>> objRespuesta = new Respuesta<IEnumerable<TEntity>>();

            try
            {

                IQueryable<TEntity> objPreconsulta = _context.Set<TEntity>();


                // Agrega los includes (relaciones) indicados, si los hay
                if (objIncludes != null)

                {

                    objIncludes.ForEach(x => objPreconsulta = objPreconsulta.Include(x));

                }


                objRespuesta.Data = await objPreconsulta.ToListAsync();

            }

            catch (Exception ex)

            {

                objRespuesta.Error = ex.Message;

                objRespuesta.Data = null;

            }

            return objRespuesta;

        }

        // Busca varias entidades que cumplan una condición (predicado), con relaciones opcionales
        public async Task<Respuesta<IEnumerable<TEntity>>> BuscarAsync(Expression<Func<TEntity, bool>> objPredicado, List<string>? objIncludes = null)

        {

            Respuesta<IEnumerable<TEntity>> objRespuesta = new Respuesta<IEnumerable<TEntity>>();

            try
            {

                IQueryable<TEntity> objPreconsulta = _context.Set<TEntity>();


                if (objIncludes != null)

                {

                    objIncludes.ForEach(x => objPreconsulta = objPreconsulta.Include(x));

                }


                objRespuesta.Data = await objPreconsulta.Where(objPredicado).ToListAsync();

            }

            catch (Exception ex)

            {

                objRespuesta.Error = ex.Message;

                objRespuesta.Data = null;

            }

            return objRespuesta;

        }


        // Obtiene una sola entidad que cumpla una condición (predicado), con relaciones opcionales
        public async Task<Respuesta<TEntity>> ObtenerEntidadAsync(Expression<Func<TEntity, bool>> objPredicado, List<string>? objIncludes = null)

        {

            Respuesta<TEntity> objRespuesta = new Respuesta<TEntity>();

            try
            {

                IQueryable<TEntity> objPreconsulta = _context.Set<TEntity>();


                if (objIncludes != null)

                {

                    objIncludes.ForEach(x => objPreconsulta = objPreconsulta.Include(x));

                }


                objRespuesta.Data = await objPreconsulta.Where(objPredicado).FirstOrDefaultAsync();

            }

            catch (Exception ex)

            {

                objRespuesta.Error = ex.Message;

                objRespuesta.Data = null;

            }

            return objRespuesta;

        }
    }
}