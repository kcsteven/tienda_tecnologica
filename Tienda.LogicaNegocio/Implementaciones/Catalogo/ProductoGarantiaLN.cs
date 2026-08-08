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
    // Implementación de la lógica de negocio (LN) para la relación Producto-Garantia
    public class ProductoGarantiaLN : IProductoGarantiaLN
    {
        // Unidad de trabajo (Entity Framework) para acceder a los repositorios de datos
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        // Logger para registrar errores y eventos de esta clase
        private ILogger<ProductoGarantiaLN> _logger { get; }
        // AutoMapper para convertir entre entidades de dominio (ProductoGarantia) y entidades tipadas (TProductoGarantia)
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias mediante inyección de dependencias
        public ProductoGarantiaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ProductoGarantiaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta una nueva asociación producto-garantía, validando que el producto y la garantía existan
        public async Task<Respuesta<TProductoGarantia>> InsertarAsync(TProductoGarantia datos)
        {
            var resultado = new Respuesta<TProductoGarantia>();
            try
            {
                // Verifica que el producto indicado exista
                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                // Verifica que la garantía indicada exista
                var garantia = await _unidadDeTrabajo.TGarantia.ObtenerEntidadAsync(x => x.GarantiaId == datos.GarantiaId);
                if (garantia.Data == null)
                {
                    resultado.Error = "La garantía indicada no existe.";
                    return resultado;
                }

                // Convierte el DTO tipado a la entidad de dominio
                var entidad = _mapper.Map<ProductoGarantia>(datos);
                // Inserta la entidad en el repositorio
                var respuesta = await _unidadDeTrabajo.TProductoGarantia.InsertarAsync(entidad);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                // Convierte la entidad insertada de vuelta a DTO tipado para la respuesta
                resultado.Data = _mapper.Map<TProductoGarantia>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Registra el error y lo devuelve en la respuesta
                _logger.LogError(ex, "Error al asociar garantía al producto {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Elimina una asociación producto-garantía existente, validando primero que exista
        public async Task<Respuesta<bool>> EliminarAsync(TProductoGarantia datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Busca el registro de asociación a eliminar por su Id
                var registro = await _unidadDeTrabajo.TProductoGarantia.ObtenerEntidadAsync(x => x.ProductoGarantiaId == datos.ProductoGarantiaId);
                if (registro.Data == null)
                {
                    resultado.Error = "No existe la asociación a eliminar.";
                    return resultado;
                }

                // Elimina la entidad del repositorio
                var respuesta = await _unidadDeTrabajo.TProductoGarantia.EliminarAsync(registro.Data);
                // Confirma (commit) los cambios en la unidad de trabajo
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

        // Lista las garantías asociadas a un producto específico
        public async Task<Respuesta<IEnumerable<TProductoGarantia>>> ListarPorProductoAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TProductoGarantia>>();
            try
            {
                // Filtra las asociaciones por el Id de producto recibido
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