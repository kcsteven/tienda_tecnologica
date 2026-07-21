using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Ventas.Dominio.Entidades;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Dominio.InterfacesAD;
using Ventas.Dominio.InterfazLN;
using Ventas.Utilidades;

namespace Ventas.LogicaNegocio.Implementaciones
{
    public class BodegaLN : IBodegaLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<BodegaLN> _logger { get; }
        private readonly IMapper _mapper;

        public BodegaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<BodegaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TBodega>> InsertarAsync(TBodega datos)
        {
            var resultado = new Respuesta<TBodega>();
            try
            {
                var existente = await _unidadDeTrabajo.TBodega.ObtenerEntidadAsync(x => x.Nombre == datos.Nombre);
                if (existente.Data != null)
                {
                    resultado.Error = "Ya existe una bodega registrada con ese nombre.";
                    return resultado;
                }

                datos.CreadoEn = DateTime.UtcNow;
                var entidad = _mapper.Map<Bodega>(datos);
                var respuesta = await _unidadDeTrabajo.TBodega.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TBodega>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar bodega {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TBodega>> ModificarAsync(TBodega datos)
        {
            var resultado = new Respuesta<TBodega>();
            try
            {
                var actual = await _unidadDeTrabajo.TBodega.ObtenerEntidadAsync(x => x.BodegaId == datos.BodegaId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe la bodega a modificar.";
                    return resultado;
                }

                datos.ActualizadoEn = DateTime.UtcNow;
                _mapper.Map(datos, actual.Data);

                var respuesta = await _unidadDeTrabajo.TBodega.ModificarAsync(actual.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TBodega>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar BodegaId {BodegaId}", datos.BodegaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TBodega datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var bodega = await _unidadDeTrabajo.TBodega.ObtenerEntidadAsync(x => x.BodegaId == datos.BodegaId);
                if (bodega.Data == null)
                {
                    resultado.Error = "No existe la bodega a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TBodega.EliminarAsync(bodega.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar BodegaId {BodegaId}", datos.BodegaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TBodega>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TBodega>>();
            try
            {
                var resp = await _unidadDeTrabajo.TBodega.ListarAsync();
                resultado.Data = _mapper.Map<IEnumerable<TBodega>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar bodegas.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TBodega>>> BuscarAsync(TBodega datos)
        {
            var resultado = new Respuesta<IEnumerable<TBodega>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TBodega.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));
                resultado.Data = _mapper.Map<IEnumerable<TBodega>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar bodegas.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TBodega>> ObtenerAsync(TBodega datos)
        {
            var resultado = new Respuesta<TBodega>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TBodega.ObtenerEntidadAsync(x => x.BodegaId == datos.BodegaId);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Bodega no encontrada.";
                    return resultado;
                }
                resultado.Data = _mapper.Map<TBodega>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener BodegaId {BodegaId}", datos.BodegaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}