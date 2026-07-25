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
    public class InventarioLN : IInventarioLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<InventarioLN> _logger { get; }
        private readonly IMapper _mapper;

        public InventarioLN(IUnidadTrabajoEF unidadTrabajo, ILogger<InventarioLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TInventario>> InsertarAsync(TInventario datos)
        {
            var resultado = new Respuesta<TInventario>();
            try
            {
                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                var bodega = await _unidadDeTrabajo.TBodega.ObtenerEntidadAsync(x => x.BodegaId == datos.BodegaId);
                if (bodega.Data == null)
                {
                    resultado.Error = "La bodega indicada no existe.";
                    return resultado;
                }

                var entidad = _mapper.Map<Inventario>(datos);
                var respuesta = await _unidadDeTrabajo.TInventario.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TInventario>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar inventario del producto {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TInventario>> ModificarAsync(TInventario datos)
        {
            var resultado = new Respuesta<TInventario>();
            try
            {
                var actual = await _unidadDeTrabajo.TInventario.ObtenerEntidadAsync(x => x.InventarioId == datos.InventarioId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe el registro de inventario a modificar.";
                    return resultado;
                }

                _mapper.Map(datos, actual.Data);

                var respuesta = await _unidadDeTrabajo.TInventario.ModificarAsync(actual.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TInventario>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar InventarioId {InventarioId}", datos.InventarioId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TInventario datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var inventario = await _unidadDeTrabajo.TInventario.ObtenerEntidadAsync(x => x.InventarioId == datos.InventarioId);
                if (inventario.Data == null)
                {
                    resultado.Error = "No existe el registro de inventario a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TInventario.EliminarAsync(inventario.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar InventarioId {InventarioId}", datos.InventarioId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TInventario>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TInventario>>();
            try
            {
                var resp = await _unidadDeTrabajo.TInventario.ListarAsync();
                resultado.Data = _mapper.Map<IEnumerable<TInventario>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar inventario.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TInventario>>> ListarPorProductoAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TInventario>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TInventario.BuscarAsync(x => x.ProductoId == productoId);
                resultado.Data = _mapper.Map<IEnumerable<TInventario>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar inventario del producto {ProductoId}", productoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TInventario>> ObtenerAsync(TInventario datos)
        {
            var resultado = new Respuesta<TInventario>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TInventario.ObtenerEntidadAsync(x => x.InventarioId == datos.InventarioId);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Registro de inventario no encontrado.";
                    return resultado;
                }
                resultado.Data = _mapper.Map<TInventario>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener InventarioId {InventarioId}", datos.InventarioId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}