using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Ventas.Dominio.InterfacesAD;
using Ventas.Utilidades;

namespace Ventas.AccesoDatos.Implementaciones
{
    public class RepositorioAD<TEntity> : IRepositorioAD<TEntity> where TEntity : class
    {

        #region Atributos y Variables


        protected readonly DbContext _context;


        public RepositorioAD(DbContext context)

        {

            this._context = context;

        }



        #endregion#region Métodos Públicos


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

                objRespuesta.Error = ex.Message;

                objRespuesta.Data = null;

            }

            return objRespuesta;

        }


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

                objRespuesta.Error = ex.Message;

                objRespuesta.Data = null;

            }

            return objRespuesta;

        }


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

                objRespuesta.Error = ex.Message;

                objRespuesta.Data = false;

            }

            return objRespuesta;

        }


        public async Task<Respuesta<IEnumerable<TEntity>>> ListarAsync(List<string>? objIncludes = null)

        {

            Respuesta<IEnumerable<TEntity>> objRespuesta = new Respuesta<IEnumerable<TEntity>>();

            try
            {

                IQueryable<TEntity> objPreconsulta = _context.Set<TEntity>();


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

