
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
    public class ResenaLN : IResenaLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<ResenaLN> _logger { get; }
        private readonly IMapper _mapper;

        public ResenaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ResenaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TResena>> InsertarAsync(TResena datos)
        {
            var resultado = new Respuesta<TResena>();
            try
            {
                if (datos.Calificacion < 1 || datos.Calificacion > 5)
                {
                    resultado.Error = "La calificación debe estar entre 1 y 5.";
                    return resultado;
                }

                var cliente = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(x => x.ClienteId == datos.ClienteId);
                if (cliente.Data == null)
                {
                    resultado.Error = "El cliente indicado no existe.";
                    return resultado;
                }

                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                datos.Fecha = DateTime.UtcNow;
                var entidad = _mapper.Map<Resena>(datos);
                var respuesta = await _unidadDeTrabajo.TResena.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TResena>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar reseña del producto {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TResena datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var resena = await _unidadDeTrabajo.TResena.ObtenerEntidadAsync(x => x.ResenaId == datos.ResenaId);
                if (resena.Data == null)
                {
                    resultado.Error = "No existe la reseña a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TResena.EliminarAsync(resena.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ResenaId {ResenaId}", datos.ResenaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TResena>>> ListarPorProductoAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TResena>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TResena.BuscarAsync(x => x.ProductoId == productoId);
                resultado.Data = _mapper.Map<IEnumerable<TResena>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar reseñas del producto {ProductoId}", productoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}