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
    public class ProductoEtiquetaLN : IProductoEtiquetaLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<ProductoEtiquetaLN> _logger { get; }
        private readonly IMapper _mapper;

        public ProductoEtiquetaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ProductoEtiquetaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TProductoEtiqueta>> InsertarAsync(TProductoEtiqueta datos)
        {
            var resultado = new Respuesta<TProductoEtiqueta>();
            try
            {
                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                var etiqueta = await _unidadDeTrabajo.TEtiqueta.ObtenerEntidadAsync(x => x.EtiquetaId == datos.EtiquetaId);
                if (etiqueta.Data == null)
                {
                    resultado.Error = "La etiqueta indicada no existe.";
                    return resultado;
                }

                var entidad = _mapper.Map<ProductoEtiqueta>(datos);
                var respuesta = await _unidadDeTrabajo.TProductoEtiqueta.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TProductoEtiqueta>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asociar etiqueta al producto {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TProductoEtiqueta datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var registro = await _unidadDeTrabajo.TProductoEtiqueta.ObtenerEntidadAsync(x => x.ProductoEtiquetaId == datos.ProductoEtiquetaId);
                if (registro.Data == null)
                {
                    resultado.Error = "No existe la asociación a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TProductoEtiqueta.EliminarAsync(registro.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ProductoEtiquetaId {Id}", datos.ProductoEtiquetaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TProductoEtiqueta>>> ListarPorProductoAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TProductoEtiqueta>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TProductoEtiqueta.BuscarAsync(x => x.ProductoId == productoId);
                resultado.Data = _mapper.Map<IEnumerable<TProductoEtiqueta>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar etiquetas del producto {ProductoId}", productoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}