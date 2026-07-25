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
    public class GarantiaLN : IGarantiaLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<GarantiaLN> _logger { get; }
        private readonly IMapper _mapper;

        public GarantiaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<GarantiaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TGarantia>> InsertarAsync(TGarantia datos)
        {
            var resultado = new Respuesta<TGarantia>();
            try
            {
                datos.CreadoEn = DateTime.UtcNow;
                var entidad = _mapper.Map<Garantia>(datos);
                var respuesta = await _unidadDeTrabajo.TGarantia.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TGarantia>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar garantía {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TGarantia>> ModificarAsync(TGarantia datos)
        {
            var resultado = new Respuesta<TGarantia>();
            try
            {
                var actual = await _unidadDeTrabajo.TGarantia.ObtenerEntidadAsync(x => x.GarantiaId == datos.GarantiaId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe la garantía a modificar.";
                    return resultado;
                }

                datos.ActualizadoEn = DateTime.UtcNow;
                _mapper.Map(datos, actual.Data);

                var respuesta = await _unidadDeTrabajo.TGarantia.ModificarAsync(actual.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TGarantia>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar GarantiaId {GarantiaId}", datos.GarantiaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TGarantia datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var garantia = await _unidadDeTrabajo.TGarantia.ObtenerEntidadAsync(x => x.GarantiaId == datos.GarantiaId);
                if (garantia.Data == null)
                {
                    resultado.Error = "No existe la garantía a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TGarantia.EliminarAsync(garantia.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar GarantiaId {GarantiaId}", datos.GarantiaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TGarantia>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TGarantia>>();
            try
            {
                var resp = await _unidadDeTrabajo.TGarantia.ListarAsync();
                resultado.Data = _mapper.Map<IEnumerable<TGarantia>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar garantías.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TGarantia>>> BuscarAsync(TGarantia datos)
        {
            var resultado = new Respuesta<IEnumerable<TGarantia>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TGarantia.BuscarAsync(x => x.Nombre != null && x.Nombre.Contains(datos.Nombre ?? ""));
                resultado.Data = _mapper.Map<IEnumerable<TGarantia>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar garantías.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TGarantia>> ObtenerAsync(TGarantia datos)
        {
            var resultado = new Respuesta<TGarantia>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TGarantia.ObtenerEntidadAsync(x => x.GarantiaId == datos.GarantiaId);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Garantía no encontrada.";
                    return resultado;
                }
                resultado.Data = _mapper.Map<TGarantia>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener GarantiaId {GarantiaId}", datos.GarantiaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}