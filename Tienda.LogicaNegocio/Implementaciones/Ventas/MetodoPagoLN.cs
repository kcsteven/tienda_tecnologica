using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.LogicaNegocio.Implementaciones;

public class MetodoPagoLN : IMetodoPagoLN
{
    private readonly IUnidadTrabajoEF _unidadDeTrabajo;
    private readonly ILogger<MetodoPagoLN> _logger;

    public MetodoPagoLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        ILogger<MetodoPagoLN> logger)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _logger = logger;
    }

    public async Task<Respuesta<IEnumerable<TMetodoPago>>> ListarAsync()
    {
        var resultado = new Respuesta<IEnumerable<TMetodoPago>>();

        try
        {
            var respuesta = await _unidadDeTrabajo.TMetodoPago.BuscarAsync(
                x => true
            );

            resultado.Data = respuesta.Data?
                .Select(x => new TMetodoPago
                {
                    MetodoPagoId = x.MetodoPagoId,
                    Nombre = x.Nombre
                })
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar métodos de pago");

            resultado.Error = ex.Message;
        }

        return resultado;
    }
}