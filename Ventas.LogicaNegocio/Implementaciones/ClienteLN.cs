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
    public class ClienteLN : IClienteLN { 
    
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }


        private ILogger<ClienteLN> _logger { get; }


        private readonly IMapper _mapper;


        public ClienteLN(

            IUnidadTrabajoEF unidadTrabajo,

            ILogger<ClienteLN> logger,

            IMapper mapper)

        {

            _unidadDeTrabajo = unidadTrabajo;

            _logger = logger;

            _mapper = mapper;

        }


        public async Task<Respuesta<TCliente>> InsertarAsync(TCliente datos)

        {

            var resultado = new Respuesta<TCliente>();


            try
            {

                var clienteExistente =

                    await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(

                        x => x.Nombre == datos.Nombre);


                if (clienteExistente.Data != null)

                {

                    resultado.Data =

                        _mapper.Map<TCliente>(clienteExistente.Data);


                    resultado.Error =

                        "Ya existe una categoría registrada con ese nombre.";


                    return resultado;

                }


                datos.CreadoEn = DateTime.UtcNow;


                var entidad = _mapper.Map<Cliente>(datos);


                var respuestaRepositorio =

                    await _unidadDeTrabajo.TCliente.InsertarAsync(entidad);


                _unidadDeTrabajo.Completar();


                resultado.Data =

                    _mapper.Map<TCliente>(respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al insertar categoría {NombreCategoria}",

                    datos.Nombre);


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<IEnumerable<TCliente>>> ListarAsync()

        {

            var resultado =

                new Respuesta<IEnumerable<TCliente>>();


            try
            {

                var resp =

                    await _unidadDeTrabajo.TCliente.ListarAsync();


                resultado.Data =

                    _mapper.Map<IEnumerable<TCliente>>(resp.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al listar categorías.");


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<TCliente>> ModificarAsync(TCliente datos)

        {

            var resultado =

                new Respuesta<TCliente>();


            try
            {

                var clienteActual =

                    await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(

                        x => x.ClienteId == datos.ClienteId);


                if (clienteActual.Data == null)

                {

                    resultado.Error =

                        "No existe la categoría a modificar.";


                    return resultado;

                }


                datos.ActualizadoEn = DateTime.UtcNow;


                _mapper.Map(datos, clienteActual.Data);


                var respuestaRepositorio =

                    await _unidadDeTrabajo.TCliente.ModificarAsync(

                        clienteActual.Data);


                _unidadDeTrabajo.Completar();


                resultado.Data =

                    _mapper.Map<TCliente>(respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al modificar CategoriaId {CategoriaId}",

                    datos.ClienteId);


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<bool>> EliminarAsync(TCliente datos)

        {

            var resultado =

                new Respuesta<bool>();


            try
            {

                var cliente =

                    await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(

                        x => x.ClienteId == datos.ClienteId);


                if (cliente.Data == null)

                {

                    resultado.Error =

                        "No existe la categoría a eliminar.";


                    return resultado;

                }


                var respuestaRepositorio =

                    await _unidadDeTrabajo.TCliente.EliminarAsync(

                        cliente.Data);


                _unidadDeTrabajo.Completar();


                resultado.Data =

                    respuestaRepositorio.Data;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al eliminar CategoriaId {CategoriaId}",

                    datos.ClienteId);


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<IEnumerable<TCliente>>> BuscarAsync(

            TCliente datos)

        {

            var resultado =

                new Respuesta<IEnumerable<TCliente>>();


            try
            {

                var respuestaRepositorio =

                    await _unidadDeTrabajo.TCliente.BuscarAsync(

                        x => x.Nombre.Contains(

                            datos.Nombre));


                resultado.Data =

                    _mapper.Map<IEnumerable<TCliente>>(

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


        public async Task<Respuesta<TCliente>> ObtenerAsync(

            TCliente datos)

        {

            var resultado =

                new Respuesta<TCliente>();


            try
            {

                var respuestaRepositorio =

                    await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(

                        x => x.ClienteId == datos.ClienteId);


                if (respuestaRepositorio.Data == null)

                {

                    resultado.Error =

                        "Cliente no encontrada.";


                    return resultado;

                }


                resultado.Data =

                    _mapper.Map<TCliente>(

                        respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al obtener CategoriaId {CategoriaId}",

                    datos.ClienteId);


                resultado.Error = ex.Message;

            }


            return resultado;

        }
    }
}
