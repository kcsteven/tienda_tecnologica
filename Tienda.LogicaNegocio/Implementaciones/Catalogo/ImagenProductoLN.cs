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
    // Implementación de la lógica de negocio (LN) para la entidad ImagenProducto
    public class ImagenProductoLN : IImagenProductoLN
    {
        // Unidad de trabajo (Entity Framework) para acceder a los repositorios de datos
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        // Logger para registrar errores y eventos de esta clase
        private ILogger<ImagenProductoLN> _logger { get; }
        // AutoMapper para convertir entre entidades de dominio (ImagenProducto) y entidades tipadas (TImagenProducto)
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias mediante inyección de dependencias
        public ImagenProductoLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ImagenProductoLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta una nueva imagen de producto, validando que el producto indicado exista
        public async Task<Respuesta<TImagenProducto>> InsertarAsync(TImagenProducto datos)
        {
            var resultado = new Respuesta<TImagenProducto>();
            try
            {
                // Verifica que el producto indicado exista
                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                // Convierte el DTO tipado a la entidad de dominio
                var entidad = _mapper.Map<ImagenProducto>(datos);
                // Inserta la entidad en el repositorio
                var respuesta = await _unidadDeTrabajo.TImagenProducto.InsertarAsync(entidad);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                // Convierte la entidad insertada de vuelta a DTO tipado para la respuesta
                resultado.Data = _mapper.Map<TImagenProducto>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Registra el error y lo devuelve en la respuesta
                _logger.LogError(ex, "Error al insertar imagen del producto {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Elimina una imagen de producto existente, validando primero que exista
        public async Task<Respuesta<bool>> EliminarAsync(TImagenProducto datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Busca la imagen a eliminar por su Id
                var imagen = await _unidadDeTrabajo.TImagenProducto.ObtenerEntidadAsync(x => x.ImagenId == datos.ImagenId);
                if (imagen.Data == null)
                {
                    resultado.Error = "No existe la imagen a eliminar.";
                    return resultado;
                }

                // Elimina la entidad del repositorio
                var respuesta = await _unidadDeTrabajo.TImagenProducto.EliminarAsync(imagen.Data);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ImagenId {ImagenId}", datos.ImagenId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Lista las imágenes asociadas a un producto específico
        public async Task<Respuesta<IEnumerable<TImagenProducto>>> ListarPorProductoAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TImagenProducto>>();
            try
            {
                // Filtra las imágenes por el Id de producto recibido
                var respuesta = await _unidadDeTrabajo.TImagenProducto.BuscarAsync(x => x.ProductoId == productoId);
                resultado.Data = _mapper.Map<IEnumerable<TImagenProducto>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar imágenes del producto {ProductoId}", productoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}