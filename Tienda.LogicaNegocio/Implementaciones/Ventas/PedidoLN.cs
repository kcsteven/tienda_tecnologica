using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.LogicaNegocio.Implementaciones
{
    public class PedidoLN : IPedidoLN { 
    
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }


        private ILogger<PedidoLN> _logger { get; }


        private readonly IMapper _mapper;


        public PedidoLN(

            IUnidadTrabajoEF unidadTrabajo,

            ILogger<PedidoLN> logger,

            IMapper mapper)

        {

            _unidadDeTrabajo = unidadTrabajo;

            _logger = logger;

            _mapper = mapper;

        }


        public async Task<Respuesta<TPedido>> InsertarAsync(TPedido datos)

        {

            var resultado = new Respuesta<TPedido>();


            try
            {

                var pedidoExistente =

                    await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(

                        x => x.NombrePedido == datos.NombrePedido);


                if (pedidoExistente.Data != null)

                {

                    resultado.Data =

                        _mapper.Map<TPedido>(pedidoExistente.Data);


                    resultado.Error =

                        "Ya existe una categoría registrada con ese nombre.";


                    return resultado;

                }


                datos.CreadoEn = DateTime.UtcNow;


                var entidad = _mapper.Map<Pedido>(datos);


                var respuestaRepositorio =

                    await _unidadDeTrabajo.TPedido.InsertarAsync(entidad);


                _unidadDeTrabajo.Completar();


                resultado.Data =

                    _mapper.Map<TPedido>(respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al insertar categoría {NombreCategoria}",

                    datos.NombrePedido);


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<IEnumerable<TPedido>>> ListarAsync()

        {

            var resultado =

                new Respuesta<IEnumerable<TPedido>>();


            try
            {

                var resp =

                    await _unidadDeTrabajo.TCategoria.ListarAsync();


                resultado.Data =

                    _mapper.Map<IEnumerable<TPedido>>(resp.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al listar categorías.");


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<TPedido>> ModificarAsync(TPedido datos)

        {

            var resultado =

                new Respuesta<TPedido>();


            try
            {

                var pedidoActual =

                    await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(

                        x => x.PedidoId == datos.PedidoId);


                if (pedidoActual.Data == null)

                {

                    resultado.Error =

                        "No existe la categoría a modificar.";


                    return resultado;

                }


                datos.ActualizadoEn = DateTime.UtcNow;


                _mapper.Map(datos, pedidoActual.Data);


                var respuestaRepositorio =

                    await _unidadDeTrabajo.TPedido.ModificarAsync(

                        pedidoActual.Data);


                _unidadDeTrabajo.Completar();


                resultado.Data =

                    _mapper.Map<TPedido>(respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al modificar CategoriaId {CategoriaId}",

                    datos.PedidoId);


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<bool>> EliminarAsync(TPedido datos)

        {

            var resultado =

                new Respuesta<bool>();


            try
            {

                var pedido =

                    await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(

                        x => x.PedidoId == datos.PedidoId);


                if (pedido.Data == null)

                {

                    resultado.Error =

                        "No existe la categoría a eliminar.";


                    return resultado;

                }


                var respuestaRepositorio =

                    await _unidadDeTrabajo.TPedido.EliminarAsync(

                        pedido.Data);


                _unidadDeTrabajo.Completar();


                resultado.Data =

                    respuestaRepositorio.Data;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al eliminar CategoriaId {CategoriaId}",

                    datos.PedidoId);


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<IEnumerable<TPedido>>> BuscarAsync(

            TPedido datos)

        {

            var resultado =

                new Respuesta<IEnumerable<TPedido>>();


            try
            {

                var respuestaRepositorio =

                    await _unidadDeTrabajo.TPedido.BuscarAsync(

                        x => x.NombrePedido.Contains(

                            datos.NombrePedido));


                resultado.Data =

                    _mapper.Map<IEnumerable<TPedido>>(

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


        public async Task<Respuesta<TPedido>> ObtenerAsync(

            TPedido datos)

        {

            var resultado =

                new Respuesta<TPedido>();


            try
            {

                var respuestaRepositorio =

                    await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(

                        x => x.PedidoId == datos.PedidoId);


                if (respuestaRepositorio.Data == null)

                {

                    resultado.Error =

                        "Categoría no encontrada.";


                    return resultado;

                }


                resultado.Data =

                    _mapper.Map<TPedido>(

                        respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al obtener CategoriaId {CategoriaId}",

                    datos.PedidoId);


                resultado.Error = ex.Message;

            }


            return resultado;

        }
    }
}
