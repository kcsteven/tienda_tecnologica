using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Tienda.Dominio.EntidadesTipadas.Usuarios;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN.Usuarios;
using Tienda.Utilidades;

// Valida y administra el acceso de los clientes
namespace Tienda.LogicaNegocio.Implementaciones;

public class ClienteLN : IClienteLN
{
    private readonly IUnidadTrabajoEF _unidadTrabajo;
    private readonly ILogger<ClienteLN> _logger;

    public ClienteLN(
        IUnidadTrabajoEF unidadTrabajo,
        ILogger<ClienteLN> logger)
    {
        _unidadTrabajo = unidadTrabajo;
        _logger = logger;
    }

    // Obtiene la información segura de los clientes
    public async Task<Respuesta<IEnumerable<TClienteAdministracion>>>
        ListarAdministracionAsync()
    {
        var resultado =
            new Respuesta<IEnumerable<TClienteAdministracion>>();

        try
        {
            resultado.Data = await _unidadTrabajo
                .ClienteAdministracion
                .ListarAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al consultar los clientes para administración.");

            resultado.Error = "No fue posible cargar los clientes.";
        }

        return resultado;
    }

    // Valida la solicitud y cambia únicamente Usuario.Activo
    public async Task<Respuesta<bool>> CambiarEstadoAsync(
        TCambiarEstadoCliente datos)
    {
        var resultado = new Respuesta<bool> { Data = false };

        if (datos is null)
        {
            resultado.Error = "Debe indicar los datos del cliente.";
            return resultado;
        }

        if (datos.ClienteId <= 0)
        {
            resultado.Error = "El cliente indicado no es válido.";
            return resultado;
        }

        if (!datos.Activo.HasValue)
        {
            resultado.Error = "Debe indicar el estado del cliente.";
            return resultado;
        }

        try
        {
            var cliente = await _unidadTrabajo
                .ClienteAdministracion
                .ObtenerAsync(datos.ClienteId);

            if (cliente is null)
            {
                resultado.Error =
                    "El cliente indicado no existe o no tiene un usuario válido.";

                return resultado;
            }

            if (cliente.Activo == datos.Activo.Value)
            {
                resultado.Data = true;
                return resultado;
            }

            var filasAfectadas = await _unidadTrabajo
                .ClienteAdministracion
                .CambiarEstadoAsync(
                    datos.ClienteId,
                    datos.Activo.Value);

            if (filasAfectadas != 1)
            {
                resultado.Error =
                    "No fue posible cambiar el estado del cliente.";

                return resultado;
            }

            resultado.Data = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al cambiar el estado del cliente {ClienteId}.",
                datos.ClienteId);

            resultado.Error =
                "No fue posible cambiar el estado del cliente.";
        }

        return resultado;
    }
}