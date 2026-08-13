using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.LogicaNegocio.Implementaciones;

public class PagoLN : IPagoLN
{
    private readonly IUnidadTrabajoEF _unidadDeTrabajo;
    private readonly ILogger<PagoLN> _logger;
    private readonly IMapper _mapper;

    public PagoLN(
        IUnidadTrabajoEF unidadTrabajo,
        ILogger<PagoLN> logger,
        IMapper mapper)
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
            var pedido = await _unidadDeTrabajo.TPedido
                .ObtenerEntidadAsync(x => x.PedidoId == datos.PedidoId);

            if (pedido.Data == null)
            {
                resultado.Error = "El pedido indicado no existe.";
                return resultado;
            }

            var metodoPago = await _unidadDeTrabajo.TMetodoPago
                .ObtenerEntidadAsync(
                    x => x.MetodoPagoId == datos.MetodoPagoId
                );

            if (metodoPago.Data == null)
            {
                resultado.Error = "El método de pago indicado no existe.";
                return resultado;
            }

            if (datos.Monto <= 0)
            {
                resultado.Error = "El monto del pago debe ser mayor que cero.";
                return resultado;
            }

            var pagoExistente = await _unidadDeTrabajo.TPago
                .BuscarAsync(x => x.PedidoId == datos.PedidoId);

            if (pagoExistente.Data != null &&
                pagoExistente.Data.Any())
            {
                resultado.Error = "Este pedido ya tiene un pago registrado.";
                return resultado;
            }

            datos.FechaPago = DateTime.Now;

            var entidad = _mapper.Map<Pago>(datos);

            var respuesta = await _unidadDeTrabajo.TPago
                .InsertarAsync(entidad);

            _unidadDeTrabajo.Completar();

            resultado.Data = _mapper.Map<TPago>(respuesta.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al registrar el pago del pedido {PedidoId}",
                datos.PedidoId
            );

            resultado.Error = ex.Message;
        }

        return resultado;
    }

    public async Task<Respuesta<bool>> EliminarAsync(TPago datos)
    {
        var resultado = new Respuesta<bool>();

        try
        {
            var pago = await _unidadDeTrabajo.TPago
                .ObtenerEntidadAsync(x => x.PagoId == datos.PagoId);

            if (pago.Data == null)
            {
                resultado.Error = "No existe el pago.";
                return resultado;
            }

            var respuesta = await _unidadDeTrabajo.TPago
                .EliminarAsync(pago.Data);

            _unidadDeTrabajo.Completar();

            resultado.Data = respuesta.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al eliminar el pago {PagoId}",
                datos.PagoId
            );

            resultado.Error = ex.Message;
        }

        return resultado;
    }

    public async Task<Respuesta<IEnumerable<TPago>>> ListarPorPedidoAsync(
        int pedidoId)
    {
        var resultado = new Respuesta<IEnumerable<TPago>>();

        try
        {
            var respuesta = await _unidadDeTrabajo.TPago
                .BuscarAsync(x => x.PedidoId == pedidoId);

            resultado.Data = respuesta.Data?
                .Select(x => new TPago
                {
                    PagoId = x.PagoId,
                    PedidoId = x.PedidoId,
                    MetodoPagoId = x.MetodoPagoId,
                    MetodoPagoNombre = x.MetodoPago?.Nombre,
                    Monto = x.Monto,
                    FechaPago = x.FechaPago,
                    Referencia = x.Referencia
                })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al listar pagos del pedido {PedidoId}",
                pedidoId
            );

            resultado.Error = ex.Message;
        }

        return resultado;
    }
}