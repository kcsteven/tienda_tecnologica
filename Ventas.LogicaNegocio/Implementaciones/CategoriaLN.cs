using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ventas.Dominio.Entidades;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Dominio.InterfacesAD;
using Ventas.Dominio.InterfazLN;
using Ventas.Utilidades;


namespace Ventas.LogicaNegocio.Implementaciones
{
    public class CategoriaLN : ICategoriaLN { 
    
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }


        private ILogger<CategoriaLN> _logger { get; }


        private readonly IMapper _mapper;


        public CategoriaLN(

            IUnidadTrabajoEF unidadTrabajo,

            ILogger<CategoriaLN> logger,

            IMapper mapper)

        {

            _unidadDeTrabajo = unidadTrabajo;

            _logger = logger;

            _mapper = mapper;

        }


        public async Task<Respuesta<TCategorium>> InsertarAsync(TCategorium datos)

        {

            var resultado = new Respuesta<TCategorium>();


            try
            {

                var categoriaExistente =

                    await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(

                        x => x.NombreCategoria == datos.NombreCategoria);


                if (categoriaExistente.Data != null)

                {

                    resultado.Data =

                        _mapper.Map<TCategorium>(categoriaExistente.Data);


                    resultado.Error =

                        "Ya existe una categoría registrada con ese nombre.";


                    return resultado;

                }


                datos.CreadoEn = DateTime.UtcNow;


                var entidad = _mapper.Map<Categorium>(datos);


                var respuestaRepositorio =

                    await _unidadDeTrabajo.TCategoria.InsertarAsync(entidad);


                _unidadDeTrabajo.Completar();


                resultado.Data =

                    _mapper.Map<TCategorium>(respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al insertar categoría {NombreCategoria}",

                    datos.NombreCategoria);


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<IEnumerable<TCategorium>>> ListarAsync()

        {

            var resultado =

                new Respuesta<IEnumerable<TCategorium>>();


            try
            {

                var resp =

                    await _unidadDeTrabajo.TCategoria.ListarAsync();


                resultado.Data =

                    _mapper.Map<IEnumerable<TCategorium>>(resp.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al listar categorías.");


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<TCategorium>> ModificarAsync(TCategorium datos)

        {

            var resultado =

                new Respuesta<TCategorium>();


            try
            {

                var categoriaActual =

                    await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(

                        x => x.CategoriaId == datos.CategoriaId);


                if (categoriaActual.Data == null)

                {

                    resultado.Error =

                        "No existe la categoría a modificar.";


                    return resultado;

                }


                datos.ActualizadoEn = DateTime.UtcNow;


                _mapper.Map(datos, categoriaActual.Data);


                var respuestaRepositorio =

                    await _unidadDeTrabajo.TCategoria.ModificarAsync(

                        categoriaActual.Data);


                _unidadDeTrabajo.Completar();


                resultado.Data =

                    _mapper.Map<TCategorium>(respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al modificar CategoriaId {CategoriaId}",

                    datos.CategoriaId);


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<bool>> EliminarAsync(TCategorium datos)

        {

            var resultado =

                new Respuesta<bool>();


            try
            {

                var categoria =

                    await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(

                        x => x.CategoriaId == datos.CategoriaId);


                if (categoria.Data == null)

                {

                    resultado.Error =

                        "No existe la categoría a eliminar.";


                    return resultado;

                }


                var respuestaRepositorio =

                    await _unidadDeTrabajo.TCategoria.EliminarAsync(

                        categoria.Data);


                _unidadDeTrabajo.Completar();


                resultado.Data =

                    respuestaRepositorio.Data;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al eliminar CategoriaId {CategoriaId}",

                    datos.CategoriaId);


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<IEnumerable<TCategorium>>> BuscarAsync(

            TCategorium datos)

        {

            var resultado =

                new Respuesta<IEnumerable<TCategorium>>();


            try
            {

                var respuestaRepositorio =

                    await _unidadDeTrabajo.TCategoria.BuscarAsync(

                        x => x.NombreCategoria.Contains(

                            datos.NombreCategoria));


                resultado.Data =

                    _mapper.Map<IEnumerable<TCategorium>>(

                        respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al buscar categorías.");


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<TCategorium>> ObtenerAsync(

            TCategorium datos)

        {

            var resultado =

                new Respuesta<TCategorium>();


            try
            {

                var respuestaRepositorio =

                    await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(

                        x => x.CategoriaId == datos.CategoriaId);


                if (respuestaRepositorio.Data == null)

                {

                    resultado.Error =

                        "Categoría no encontrada.";


                    return resultado;

                }


                resultado.Data =

                    _mapper.Map<TCategorium>(

                        respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al obtener CategoriaId {CategoriaId}",

                    datos.CategoriaId);


                resultado.Error = ex.Message;

            }


            return resultado;

        }
    }
}
