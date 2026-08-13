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

public class AccesoUsuarioLN : IAccesoUsuarioLN
{
    private const string ErrorCredenciales = "Correo o contraseña incorrectos";
    private readonly IUnidadTrabajoEF _unidadDeTrabajo;
    private readonly IHashContrasena _hashContrasena;
    private readonly ITokenSesion _tokenSesion;
    private readonly ILogger<AccesoUsuarioLN> _logger;

    public AccesoUsuarioLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IHashContrasena hashContrasena,
        ITokenSesion tokenSesion,
        ILogger<AccesoUsuarioLN> logger)
    {

        _unidadDeTrabajo = unidadDeTrabajo;
        _hashContrasena = hashContrasena;
        _tokenSesion = tokenSesion;
        _logger = logger;
    }

    //Valida credenciales para generar la sesion
    public async Task<Respuesta<TSesionUsuario>> IniciarSesionAsync(TInicioSesion datos)
    {
        var resultado = new Respuesta<TSesionUsuario>();
        if (datos is null || string.IsNullOrWhiteSpace(datos.Email) || string.IsNullOrEmpty(datos.Contrasena))
        {
            resultado.Error = ErrorCredenciales;
            return resultado;
        }

        var emailNormalizado = datos.Email.Trim().ToLowerInvariant();

        try
        {
            // La busqueda y el error de correo y contra son genericos para no revelar correos existentes
            var respuestaUsuario = await _unidadDeTrabajo.TUsuario.ObtenerEntidadAsync(
                usuario => usuario.Persona.Email == emailNormalizado,
                new List<string> { nameof(Usuario.Persona), nameof(Usuario.Rol) });

            if (!string.IsNullOrEmpty(respuestaUsuario.Error))
            {
                _logger.LogError("Error técnico al buscar el usuario para inicio de sesión");
                resultado.Error = "No fue posible iniciar sesión";
                return resultado;
            }

            var usuario = respuestaUsuario.Data;
            if (usuario is null || !_hashContrasena.VerificarHash(datos.Contrasena, usuario.Contrasena))
            {
                resultado.Error = ErrorCredenciales;
                return resultado;
            }

            if (!usuario.Activo)
            {
                resultado.Error = "La cuenta se encuentra inactiva";
                return resultado;
            }

            if (usuario.Persona is null || usuario.Rol is null)
            {
                _logger.LogError("El usuario autenticado no tiene sus datos o Rol cargados correctamente");
                resultado.Error = "No fue posible iniciar sesión";
                return resultado;
            }

            var rol = ObtenerRolCanonico(usuario.Rol.Nombre);
            if (rol is null)
            {
                resultado.Error = "La cuenta no tiene un rol autorizado";
                return resultado;
            }

            var nombreCompleto = string.Join(
                " ",
                new[] { usuario.Persona.Nombre, usuario.Persona.Apellido }
                    .Where(nombre => !string.IsNullOrWhiteSpace(nombre))
                    .Select(nombre => nombre.Trim()));

            // Si el rol es Cliente, resolvemos su ClienteId real para incluirlo en la sesión
            int? clienteId = null;
            if (rol == "Cliente")
            {
                var respuestaCliente = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(
                    cliente => cliente.PersonaId == usuario.PersonaId);

                if (!string.IsNullOrEmpty(respuestaCliente.Error))
                {
                    _logger.LogError("Error técnico al buscar el cliente asociado al usuario para inicio de sesión");
                    resultado.Error = "No fue posible iniciar sesión";
                    return resultado;
                }

                if (respuestaCliente.Data is null)
                {
                    _logger.LogError("El usuario con rol Cliente no tiene un registro de Cliente asociado");
                    resultado.Error = "No fue posible iniciar sesión";
                    return resultado;
                }

                clienteId = respuestaCliente.Data.ClienteId;
            }

            resultado.Data = _tokenSesion.CrearSesion(
                usuario.UsuarioId,
                usuario.PersonaId,
                usuario.NombreUsuario.Trim(),
                nombreCompleto,
                emailNormalizado,
                rol,
                clienteId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error técnico durante el inicio de sesión");
            resultado.Error = "No fue posible iniciar sesión";
        }

        return resultado;
    }

    //Convierte el rol almacenado al valor permitido por la autorización
    private static string? ObtenerRolCanonico(string? rol)
    {

        if (string.Equals(rol?.Trim(), "Cliente", StringComparison.OrdinalIgnoreCase))
        {
            return "Cliente";
        }

        if (string.Equals(rol?.Trim(), "Empleado", StringComparison.OrdinalIgnoreCase))
        {
            return "Empleado";

        }

        return null;
    }
}