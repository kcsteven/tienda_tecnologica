// DevolucionLN.cs
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
    public class DevolucionLN : IDevolucionLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<DevolucionLN> _logger { get; }
        private readonly IMapper _mapper;

        public DevolucionLN(IUnidadTrabajoEF unidadTrabajo, ILogger<DevolucionLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TDevolucion>> InsertarAsync(TDevolucion datos)
        {
            var resultado = new Respuesta<TDevolucion>();
            try
            {
                var pedido = await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(x => x.PedidoId == datos.PedidoId);
                if (pedido.Data == null)
                {
                    resultado.Error = "El pedido indicado no existe.";
                    return resultado;
                }

                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                datos.Fecha = DateTime.UtcNow;
                datos.EstadoDevolucion ??= "Pendiente";
                var entidad = _mapper.Map<Devolucion>(datos);
                var respuesta = await _unidadDeTrabajo.TDevolucion.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TDevolucion>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar devolución del pedido {PedidoId}", datos.PedidoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TDevolucion>> ModificarAsync(TDevolucion datos)
        {
            var resultado = new Respuesta<TDevolucion>();
            try
            {
                var actual = await _unidadDeTrabajo.TDevolucion.ObtenerEntidadAsync(x => x.DevolucionId == datos.DevolucionId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe la devolución a modificar.";
                    return resultado;
                }

                _mapper.Map(datos, actual.Data);

                var respuesta = await _unidadDeTrabajo.TDevolucion.ModificarAsync(actual.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TDevolucion>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar DevolucionId {DevolucionId}", datos.DevolucionId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TDevolucion>>> ListarPorPedidoAsync(int pedidoId)
        {
            var resultado = new Respuesta<IEnumerable<TDevolucion>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TDevolucion.BuscarAsync(x => x.PedidoId == pedidoId);
                resultado.Data = _mapper.Map<IEnumerable<TDevolucion>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar devoluciones del pedido {PedidoId}", pedidoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}