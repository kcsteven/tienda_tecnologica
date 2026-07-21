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
    public class ProductoDescuentoLN : IProductoDescuentoLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<ProductoDescuentoLN> _logger { get; }
        private readonly IMapper _mapper;

        public ProductoDescuentoLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ProductoDescuentoLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TProductoDescuento>> InsertarAsync(TProductoDescuento datos)
        {
            var resultado = new Respuesta<TProductoDescuento>();
            try
            {
                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                var descuento = await _unidadDeTrabajo.TDescuento.ObtenerEntidadAsync(x => x.DescuentoId == datos.DescuentoId);
                if (descuento.Data == null)
                {
                    resultado.Error = "El descuento indicado no existe.";
                    return resultado;
                }

                var entidad = _mapper.Map<ProductoDescuento>(datos);
                var respuesta = await _unidadDeTrabajo.TProductoDescuento.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TProductoDescuento>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asociar descuento al producto {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TProductoDescuento datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var registro = await _unidadDeTrabajo.TProductoDescuento.ObtenerEntidadAsync(x => x.ProductoDescuentoId == datos.ProductoDescuentoId);
                if (registro.Data == null)
                {
                    resultado.Error = "No existe la asociación a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TProductoDescuento.EliminarAsync(registro.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ProductoDescuentoId {Id}", datos.ProductoDescuentoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TProductoDescuento>>> ListarPorProductoAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TProductoDescuento>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TProductoDescuento.BuscarAsync(x => x.ProductoId == productoId);
                resultado.Data = _mapper.Map<IEnumerable<TProductoDescuento>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar descuentos del producto {ProductoId}", productoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}