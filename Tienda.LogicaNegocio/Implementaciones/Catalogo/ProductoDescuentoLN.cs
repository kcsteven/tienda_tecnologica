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
    // Implementación de la lógica de negocio (LN) para la relación Producto-Descuento
    public class ProductoDescuentoLN : IProductoDescuentoLN
    {
        // Unidad de trabajo (Entity Framework) para acceder a los repositorios de datos
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        // Logger para registrar errores y eventos de esta clase
        private ILogger<ProductoDescuentoLN> _logger { get; }
        // AutoMapper para convertir entre entidades de dominio (ProductoDescuento) y entidades tipadas (TProductoDescuento)
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias mediante inyección de dependencias
        public ProductoDescuentoLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ProductoDescuentoLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta una nueva asociación producto-descuento, validando que el producto y el descuento existan
        public async Task<Respuesta<TProductoDescuento>> InsertarAsync(TProductoDescuento datos)
        {
            var resultado = new Respuesta<TProductoDescuento>();
            try
            {
                // Verifica que el producto indicado exista
                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                // Verifica que el descuento indicado exista
                var descuento = await _unidadDeTrabajo.TDescuento.ObtenerEntidadAsync(x => x.DescuentoId == datos.DescuentoId);
                if (descuento.Data == null)
                {
                    resultado.Error = "El descuento indicado no existe.";
                    return resultado;
                }

                // Convierte el DTO tipado a la entidad de dominio
                var entidad = _mapper.Map<ProductoDescuento>(datos);
                // Inserta la entidad en el repositorio
                var respuesta = await _unidadDeTrabajo.TProductoDescuento.InsertarAsync(entidad);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                // Convierte la entidad insertada de vuelta a DTO tipado para la respuesta
                resultado.Data = _mapper.Map<TProductoDescuento>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Registra el error y lo devuelve en la respuesta
                _logger.LogError(ex, "Error al asociar descuento al producto {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Elimina una asociación producto-descuento existente, validando primero que exista
        public async Task<Respuesta<bool>> EliminarAsync(TProductoDescuento datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Busca el registro de asociación a eliminar por su Id
                var registro = await _unidadDeTrabajo.TProductoDescuento.ObtenerEntidadAsync(x => x.ProductoDescuentoId == datos.ProductoDescuentoId);
                if (registro.Data == null)
                {
                    resultado.Error = "No existe la asociación a eliminar.";
                    return resultado;
                }

                // Elimina la entidad del repositorio
                var respuesta = await _unidadDeTrabajo.TProductoDescuento.EliminarAsync(registro.Data);
                // Confirma (commit) los cambios en la unidad de trabajo
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

        // Lista los descuentos asociados a un producto específico
        public async Task<Respuesta<IEnumerable<TProductoDescuento>>> ListarPorProductoAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TProductoDescuento>>();
            try
            {
                // Filtra las asociaciones por el Id de producto recibido
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