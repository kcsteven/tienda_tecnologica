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
    public class ImagenProductoLN : IImagenProductoLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<ImagenProductoLN> _logger { get; }
        private readonly IMapper _mapper;

        public ImagenProductoLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ImagenProductoLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TImagenProducto>> InsertarAsync(TImagenProducto datos)
        {
            var resultado = new Respuesta<TImagenProducto>();
            try
            {
                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                var entidad = _mapper.Map<ImagenProducto>(datos);
                var respuesta = await _unidadDeTrabajo.TImagenProducto.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TImagenProducto>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar imagen del producto {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TImagenProducto datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var imagen = await _unidadDeTrabajo.TImagenProducto.ObtenerEntidadAsync(x => x.ImagenId == datos.ImagenId);
                if (imagen.Data == null)
                {
                    resultado.Error = "No existe la imagen a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TImagenProducto.EliminarAsync(imagen.Data);
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

        public async Task<Respuesta<IEnumerable<TImagenProducto>>> ListarPorProductoAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TImagenProducto>>();
            try
            {
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