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
    public class EtiquetaLN : IEtiquetaLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<EtiquetaLN> _logger { get; }
        private readonly IMapper _mapper;

        public EtiquetaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<EtiquetaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TEtiqueta>> InsertarAsync(TEtiqueta datos)
        {
            var resultado = new Respuesta<TEtiqueta>();
            try
            {
                var existente = await _unidadDeTrabajo.TEtiqueta.ObtenerEntidadAsync(x => x.Nombre == datos.Nombre);
                if (existente.Data != null)
                {
                    resultado.Error = "Ya existe una etiqueta registrada con ese nombre.";
                    return resultado;
                }

                datos.CreadoEn = DateTime.UtcNow;
                var entidad = _mapper.Map<Etiqueta>(datos);
                var respuesta = await _unidadDeTrabajo.TEtiqueta.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TEtiqueta>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar etiqueta {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TEtiqueta>> ModificarAsync(TEtiqueta datos)
        {
            var resultado = new Respuesta<TEtiqueta>();
            try
            {
                var actual = await _unidadDeTrabajo.TEtiqueta.ObtenerEntidadAsync(x => x.EtiquetaId == datos.EtiquetaId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe la etiqueta a modificar.";
                    return resultado;
                }

                datos.ActualizadoEn = DateTime.UtcNow;
                _mapper.Map(datos, actual.Data);

                var respuesta = await _unidadDeTrabajo.TEtiqueta.ModificarAsync(actual.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TEtiqueta>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar EtiquetaId {EtiquetaId}", datos.EtiquetaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TEtiqueta datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var etiqueta = await _unidadDeTrabajo.TEtiqueta.ObtenerEntidadAsync(x => x.EtiquetaId == datos.EtiquetaId);
                if (etiqueta.Data == null)
                {
                    resultado.Error = "No existe la etiqueta a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TEtiqueta.EliminarAsync(etiqueta.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar EtiquetaId {EtiquetaId}", datos.EtiquetaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TEtiqueta>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TEtiqueta>>();
            try
            {
                var resp = await _unidadDeTrabajo.TEtiqueta.ListarAsync();
                resultado.Data = _mapper.Map<IEnumerable<TEtiqueta>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar etiquetas.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TEtiqueta>>> BuscarAsync(TEtiqueta datos)
        {
            var resultado = new Respuesta<IEnumerable<TEtiqueta>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TEtiqueta.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));
                resultado.Data = _mapper.Map<IEnumerable<TEtiqueta>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar etiquetas.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TEtiqueta>> ObtenerAsync(TEtiqueta datos)
        {
            var resultado = new Respuesta<TEtiqueta>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TEtiqueta.ObtenerEntidadAsync(x => x.EtiquetaId == datos.EtiquetaId);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Etiqueta no encontrada.";
                    return resultado;
                }
                resultado.Data = _mapper.Map<TEtiqueta>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener EtiquetaId {EtiquetaId}", datos.EtiquetaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}