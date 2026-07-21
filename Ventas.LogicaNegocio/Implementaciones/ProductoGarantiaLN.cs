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
    public class ProductoGarantiaLN : IProductoGarantiaLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<ProductoGarantiaLN> _logger { get; }
        private readonly IMapper _mapper;

        public ProductoGarantiaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ProductoGarantiaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TProductoGarantia>> InsertarAsync(TProductoGarantia datos)
        {
            var resultado = new Respuesta<TProductoGarantia>();
            try
            {
                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                var garantia = await _unidadDeTrabajo.TGarantia.ObtenerEntidadAsync(x => x.GarantiaId == datos.GarantiaId);
                if (garantia.Data == null)
                {
                    resultado.Error = "La garantía indicada no existe.";
                    return resultado;
                }

                var entidad = _mapper.Map<ProductoGarantia>(datos);
                var respuesta = await _unidadDeTrabajo.TProductoGarantia.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TProductoGarantia>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asociar garantía al producto {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TProductoGarantia datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var registro = await _unidadDeTrabajo.TProductoGarantia.ObtenerEntidadAsync(x => x.ProductoGarantiaId == datos.ProductoGarantiaId);
                if (registro.Data == null)
                {
                    resultado.Error = "No existe la asociación a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TProductoGarantia.EliminarAsync(registro.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ProductoGarantiaId {Id}", datos.ProductoGarantiaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TProductoGarantia>>> ListarPorProductoAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TProductoGarantia>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TProductoGarantia.BuscarAsync(x => x.ProductoId == productoId);
                resultado.Data = _mapper.Map<IEnumerable<TProductoGarantia>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar garantías del producto {ProductoId}", productoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}