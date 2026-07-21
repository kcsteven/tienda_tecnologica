
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
    public class PagoLN : IPagoLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<PagoLN> _logger { get; }
        private readonly IMapper _mapper;

        public PagoLN(IUnidadTrabajoEF unidadTrabajo, ILogger<PagoLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TPago>> InsertarAsync(TPago datos)
        {
            var resultado = new Respuesta<TPago>();
            try
            {
                var pedido = await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(x => x.PedidoId == datos.PedidoId);
                if (pedido.Data == null)
                {
                    resultado.Error = "El pedido indicado no existe.";
                    return resultado;
                }

                datos.FechaPago = DateTime.UtcNow;
                var entidad = _mapper.Map<Pago>(datos);
                var respuesta = await _unidadDeTrabajo.TPago.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TPago>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar pago del pedido {PedidoId}", datos.PedidoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TPago datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var pago = await _unidadDeTrabajo.TPago.ObtenerEntidadAsync(x => x.PagoId == datos.PagoId);
                if (pago.Data == null)
                {
                    resultado.Error = "No existe el pago a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TPago.EliminarAsync(pago.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar PagoId {PagoId}", datos.PagoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TPago>>> ListarPorPedidoAsync(int pedidoId)
        {
            var resultado = new Respuesta<IEnumerable<TPago>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TPago.BuscarAsync(x => x.PedidoId == pedidoId);
                resultado.Data = _mapper.Map<IEnumerable<TPago>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar pagos del pedido {PedidoId}", pedidoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}