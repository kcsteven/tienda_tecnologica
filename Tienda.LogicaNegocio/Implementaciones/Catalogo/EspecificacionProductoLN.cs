using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.LogicaNegocio.Implementaciones
{
    public class EspecificacionProductoLN : IEspecificacionProductoLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<EspecificacionProductoLN> _logger { get; }
        private readonly IMapper _mapper;

        public EspecificacionProductoLN(IUnidadTrabajoEF unidadTrabajo, ILogger<EspecificacionProductoLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TEspecificacionProducto>> InsertarAsync(TEspecificacionProducto datos)
        {
            var resultado = new Respuesta<TEspecificacionProducto>();
            try
            {
                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                var entidad = _mapper.Map<EspecificacionProducto>(datos);
                var respuesta = await _unidadDeTrabajo.TEspecificacionProducto.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TEspecificacionProducto>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar especificación del producto {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TEspecificacionProducto datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var registro = await _unidadDeTrabajo.TEspecificacionProducto.ObtenerEntidadAsync(x => x.EspecificacionId == datos.EspecificacionId);
                if (registro.Data == null)
                {
                    resultado.Error = "No existe la especificación a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TEspecificacionProducto.EliminarAsync(registro.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar EspecificacionId {Id}", datos.EspecificacionId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TEspecificacionProducto>>> ListarPorProductoAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TEspecificacionProducto>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TEspecificacionProducto.BuscarAsync(x => x.ProductoId == productoId);
                var ordenado = (respuesta.Data ?? Enumerable.Empty<EspecificacionProducto>()).OrderBy(e => e.Orden);
                resultado.Data = _mapper.Map<IEnumerable<TEspecificacionProducto>>(ordenado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar especificaciones del producto {ProductoId}", productoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}