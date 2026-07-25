using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.LogicaNegocio.Implementaciones
{
    public class ClienteLN : IClienteLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<ClienteLN> _logger { get; }
        private readonly IMapper _mapper;

        public ClienteLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ClienteLN> logger, IMapper mapper)
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
                var existente = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(x => x.Email == datos.Email);
                if (existente.Data != null)
                {
                    resultado.Error = "Ya existe un cliente registrado con ese correo.";
                    return resultado;
                }

                var tipoCedula = await _unidadDeTrabajo.TTipoCedula.ObtenerEntidadAsync(x => x.TipoCedula1 == datos.TipoCedula);
                if (tipoCedula.Data == null)
                {
                    resultado.Error = "El tipo de cédula indicado no existe.";
                    return resultado;
                }

                datos.CreadoEn = DateTime.UtcNow;
                datos.FechaRegistro = DateTime.UtcNow;
                var entidad = _mapper.Map<Cliente>(datos);
                var respuesta = await _unidadDeTrabajo.TCliente.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TCliente>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar cliente {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TCliente>> ModificarAsync(TCliente datos)
        {
            var resultado = new Respuesta<TCliente>();
            try
            {
                var actual = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(x => x.ClienteId == datos.ClienteId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe el cliente a modificar.";
                    return resultado;
                }

                datos.ActualizadoEn = DateTime.UtcNow;
                _mapper.Map(datos, actual.Data);

                var respuesta = await _unidadDeTrabajo.TCliente.ModificarAsync(actual.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TCliente>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar ClienteId {ClienteId}", datos.ClienteId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TCliente datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var cliente = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(x => x.ClienteId == datos.ClienteId);
                if (cliente.Data == null)
                {
                    resultado.Error = "No existe el cliente a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TCliente.EliminarAsync(cliente.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ClienteId {ClienteId}", datos.ClienteId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TCliente>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TCliente>>();
            try
            {
                var resp = await _unidadDeTrabajo.TCliente.ListarAsync();
                resultado.Data = _mapper.Map<IEnumerable<TCliente>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar clientes.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TCliente>>> BuscarAsync(TCliente datos)
        {
            var resultado = new Respuesta<IEnumerable<TCliente>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TCliente.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));
                resultado.Data = _mapper.Map<IEnumerable<TCliente>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar clientes.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TCliente>> ObtenerAsync(TCliente datos)
        {
            var resultado = new Respuesta<TCliente>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(x => x.ClienteId == datos.ClienteId);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Cliente no encontrado.";
                    return resultado;
                }
                resultado.Data = _mapper.Map<TCliente>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ClienteId {ClienteId}", datos.ClienteId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}