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
    public class DescuentoLN : IDescuentoLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<DescuentoLN> _logger { get; }
        private readonly IMapper _mapper;

        public DescuentoLN(IUnidadTrabajoEF unidadTrabajo, ILogger<DescuentoLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TDescuento>> InsertarAsync(TDescuento datos)
        {
            var resultado = new Respuesta<TDescuento>();
            try
            {
                var existente = await _unidadDeTrabajo.TDescuento.ObtenerEntidadAsync(x => x.Nombre == datos.Nombre);
                if (existente.Data != null)
                {
                    resultado.Error = "Ya existe un descuento registrado con ese nombre.";
                    return resultado;
                }

                datos.CreadoEn = DateTime.UtcNow;
                var entidad = _mapper.Map<Descuento>(datos);
                var respuesta = await _unidadDeTrabajo.TDescuento.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TDescuento>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar descuento {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TDescuento>> ModificarAsync(TDescuento datos)
        {
            var resultado = new Respuesta<TDescuento>();
            try
            {
                var actual = await _unidadDeTrabajo.TDescuento.ObtenerEntidadAsync(x => x.DescuentoId == datos.DescuentoId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe el descuento a modificar.";
                    return resultado;
                }

                datos.ActualizadoEn = DateTime.UtcNow;
                _mapper.Map(datos, actual.Data);

                var respuesta = await _unidadDeTrabajo.TDescuento.ModificarAsync(actual.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TDescuento>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar DescuentoId {DescuentoId}", datos.DescuentoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TDescuento datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var descuento = await _unidadDeTrabajo.TDescuento.ObtenerEntidadAsync(x => x.DescuentoId == datos.DescuentoId);
                if (descuento.Data == null)
                {
                    resultado.Error = "No existe el descuento a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TDescuento.EliminarAsync(descuento.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar DescuentoId {DescuentoId}", datos.DescuentoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TDescuento>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TDescuento>>();
            try
            {
                var resp = await _unidadDeTrabajo.TDescuento.ListarAsync();
                resultado.Data = _mapper.Map<IEnumerable<TDescuento>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar descuentos.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TDescuento>>> BuscarAsync(TDescuento datos)
        {
            var resultado = new Respuesta<IEnumerable<TDescuento>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TDescuento.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));
                resultado.Data = _mapper.Map<IEnumerable<TDescuento>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar descuentos.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TDescuento>> ObtenerAsync(TDescuento datos)
        {
            var resultado = new Respuesta<TDescuento>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TDescuento.ObtenerEntidadAsync(x => x.DescuentoId == datos.DescuentoId);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Descuento no encontrado.";
                    return resultado;
                }
                resultado.Data = _mapper.Map<TDescuento>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener DescuentoId {DescuentoId}", datos.DescuentoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}