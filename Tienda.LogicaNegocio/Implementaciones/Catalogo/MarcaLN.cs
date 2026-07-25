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
    public class MarcaLN : IMarcaLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<MarcaLN> _logger { get; }
        private readonly IMapper _mapper;

        public MarcaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<MarcaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TMarca>> InsertarAsync(TMarca datos)
        {
            var resultado = new Respuesta<TMarca>();
            try
            {
                var existente = await _unidadDeTrabajo.TMarca.ObtenerEntidadAsync(x => x.Nombre == datos.Nombre);
                if (existente.Data != null)
                {
                    resultado.Error = "Ya existe una marca registrada con ese nombre.";
                    return resultado;
                }

                datos.CreadoEn = DateTime.UtcNow;
                var entidad = _mapper.Map<Marca>(datos);
                var respuesta = await _unidadDeTrabajo.TMarca.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TMarca>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar marca {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TMarca>> ModificarAsync(TMarca datos)
        {
            var resultado = new Respuesta<TMarca>();
            try
            {
                var actual = await _unidadDeTrabajo.TMarca.ObtenerEntidadAsync(x => x.MarcaId == datos.MarcaId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe la marca a modificar.";
                    return resultado;
                }

                datos.ActualizadoEn = DateTime.UtcNow;
                _mapper.Map(datos, actual.Data);

                var respuesta = await _unidadDeTrabajo.TMarca.ModificarAsync(actual.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TMarca>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar MarcaId {MarcaId}", datos.MarcaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TMarca datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var marca = await _unidadDeTrabajo.TMarca.ObtenerEntidadAsync(x => x.MarcaId == datos.MarcaId);
                if (marca.Data == null)
                {
                    resultado.Error = "No existe la marca a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TMarca.EliminarAsync(marca.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar MarcaId {MarcaId}", datos.MarcaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TMarca>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TMarca>>();
            try
            {
                var resp = await _unidadDeTrabajo.TMarca.ListarAsync();
                resultado.Data = _mapper.Map<IEnumerable<TMarca>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar marcas.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TMarca>>> BuscarAsync(TMarca datos)
        {
            var resultado = new Respuesta<IEnumerable<TMarca>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TMarca.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));
                resultado.Data = _mapper.Map<IEnumerable<TMarca>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar marcas.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TMarca>> ObtenerAsync(TMarca datos)
        {
            var resultado = new Respuesta<TMarca>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TMarca.ObtenerEntidadAsync(x => x.MarcaId == datos.MarcaId);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Marca no encontrada.";
                    return resultado;
                }
                resultado.Data = _mapper.Map<TMarca>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener MarcaId {MarcaId}", datos.MarcaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}