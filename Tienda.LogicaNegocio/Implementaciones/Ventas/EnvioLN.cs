
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.LogicaNegocio.Implementaciones
{
    public class EnvioLN : IEnvioLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<EnvioLN> _logger { get; }
        private readonly IMapper _mapper;

        public EnvioLN(IUnidadTrabajoEF unidadTrabajo, ILogger<EnvioLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TEnvio>> InsertarAsync(TEnvio datos)
        {
            var resultado = new Respuesta<TEnvio>();
            try
            {
                var pedido = await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(x => x.PedidoId == datos.PedidoId);
                if (pedido.Data == null)
                {
                    resultado.Error = "El pedido indicado no existe.";
                    return resultado;
                }

                var entidad = _mapper.Map<Envio>(datos);
                var respuesta = await _unidadDeTrabajo.TEnvio.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TEnvio>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar envío del pedido {PedidoId}", datos.PedidoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TEnvio>> ModificarAsync(TEnvio datos)
        {
            var resultado = new Respuesta<TEnvio>();
            try
            {
                var actual = await _unidadDeTrabajo.TEnvio.ObtenerEntidadAsync(x => x.EnvioId == datos.EnvioId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe el envío a modificar.";
                    return resultado;
                }

                _mapper.Map(datos, actual.Data);

                var respuesta = await _unidadDeTrabajo.TEnvio.ModificarAsync(actual.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TEnvio>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar EnvioId {EnvioId}", datos.EnvioId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TEnvio>> ObtenerPorPedidoAsync(int pedidoId)
        {
            var resultado = new Respuesta<TEnvio>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TEnvio.BuscarAsync(x => x.PedidoId == pedidoId);
                var envio = respuesta.Data?.FirstOrDefault();
                if (envio == null)
                {
                    resultado.Error = "No hay envío registrado para este pedido.";
                    return resultado;
                }
                resultado.Data = _mapper.Map<TEnvio>(envio);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener envío del pedido {PedidoId}", pedidoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}