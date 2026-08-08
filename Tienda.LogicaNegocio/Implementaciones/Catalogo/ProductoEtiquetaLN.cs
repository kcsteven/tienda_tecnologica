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
    // Implementación de la lógica de negocio (LN) para la relación Producto-Etiqueta
    public class ProductoEtiquetaLN : IProductoEtiquetaLN
    {
        // Unidad de trabajo (Entity Framework) para acceder a los repositorios de datos
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        // Logger para registrar errores y eventos de esta clase
        private ILogger<ProductoEtiquetaLN> _logger { get; }
        // AutoMapper para convertir entre entidades de dominio (ProductoEtiqueta) y entidades tipadas (TProductoEtiqueta)
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias mediante inyección de dependencias
        public ProductoEtiquetaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ProductoEtiquetaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta una nueva asociación producto-etiqueta, validando que el producto y la etiqueta existan
        public async Task<Respuesta<TProductoEtiqueta>> InsertarAsync(TProductoEtiqueta datos)
        {
            var resultado = new Respuesta<TProductoEtiqueta>();
            try
            {
                // Verifica que el producto indicado exista
                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                // Verifica que la etiqueta indicada exista
                var etiqueta = await _unidadDeTrabajo.TEtiqueta.ObtenerEntidadAsync(x => x.EtiquetaId == datos.EtiquetaId);
                if (etiqueta.Data == null)
                {
                    resultado.Error = "La etiqueta indicada no existe.";
                    return resultado;
                }

                // Convierte el DTO tipado a la entidad de dominio
                var entidad = _mapper.Map<ProductoEtiqueta>(datos);
                // Inserta la entidad en el repositorio
                var respuesta = await _unidadDeTrabajo.TProductoEtiqueta.InsertarAsync(entidad);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                // Convierte la entidad insertada de vuelta a DTO tipado para la respuesta
                resultado.Data = _mapper.Map<TProductoEtiqueta>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Registra el error y lo devuelve en la respuesta
                _logger.LogError(ex, "Error al asociar etiqueta al producto {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Elimina una asociación producto-etiqueta existente, validando primero que exista
        public async Task<Respuesta<bool>> EliminarAsync(TProductoEtiqueta datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Busca el registro de asociación a eliminar por su Id
                var registro = await _unidadDeTrabajo.TProductoEtiqueta.ObtenerEntidadAsync(x => x.ProductoEtiquetaId == datos.ProductoEtiquetaId);
                if (registro.Data == null)
                {
                    resultado.Error = "No existe la asociación a eliminar.";
                    return resultado;
                }

                // Elimina la entidad del repositorio
                var respuesta = await _unidadDeTrabajo.TProductoEtiqueta.EliminarAsync(registro.Data);
                // Confirma (commit) los cambios en la unidad de trabajo
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

        // Lista las etiquetas asociadas a un producto específico
        public async Task<Respuesta<IEnumerable<TProductoEtiqueta>>> ListarPorProductoAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TProductoEtiqueta>>();
            try
            {
                // Filtra las asociaciones por el Id de producto recibido
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