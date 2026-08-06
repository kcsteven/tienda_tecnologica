using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.LogicaNegocio.Implementaciones;

public class RegistroClienteLN : IRegistroClienteLN
{
    private readonly IUnidadTrabajoEF _unidadDeTrabajo;
    private readonly IHashContrasena _hashContrasena;
    private readonly ILogger<RegistroClienteLN> _logger;

    public RegistroClienteLN(
        IUnidadTrabajoEF unidadDeTrabajo,
        IHashContrasena hashContrasena,
        ILogger<RegistroClienteLN> logger)
    {
        _unidadDeTrabajo = unidadDeTrabajo;
        _hashContrasena = hashContrasena;
        _logger = logger;
    }

    public async Task<Respuesta<IEnumerable<TTipoDocumento>>> ListarTiposDocumentoAsync()
    {
        var resultado = new Respuesta<IEnumerable<TTipoDocumento>>();

        try
        {
            var respuesta = await _unidadDeTrabajo.TTipoDocumento.ListarAsync();
            if (!string.IsNullOrEmpty(respuesta.Error))
            {
                resultado.Error = "No fue posible cargar los tipos de documento.";
                return resultado;
            }

            resultado.Data = respuesta.Data?
                .OrderBy(x => x.Nombre)
                .Select(x => new TTipoDocumento
                {
                    TipoDocumentoId = x.TipoDocumentoId,
                    Nombre = x.Nombre
                })
                .ToList() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar los tipos de documento.");
            resultado.Error = "No fue posible cargar los tipos de documento.";
        }

        return resultado;
    }

    public async Task<Respuesta<bool>> RegistrarAsync(TRegistroCliente datos)
    {
        var resultado = new Respuesta<bool> { Data = false };

        if (!TryNormalizarDatos(datos, out var datosNormalizados, out var error))
        {
            resultado.Error = error;
            return resultado;
        }

        try
        {
            var tipoDocumento = await _unidadDeTrabajo.TTipoDocumento.ObtenerEntidadAsync(
                x => x.TipoDocumentoId == datos.TipoDocumentoId);
            if (!string.IsNullOrEmpty(tipoDocumento.Error))
            {
                resultado.Error = "No fue posible validar el tipo de documento.";
                return resultado;
            }

            if (tipoDocumento.Data == null)
            {
                resultado.Error = "El tipo de documento indicado no existe.";
                return resultado;
            }

            if (!EsDocumentoValido(tipoDocumento.Data.Nombre, datosNormalizados.NumeroDocumento, out error))
            {
                resultado.Error = error;
                return resultado;
            }

            datosNormalizados = datosNormalizados with
            {
                NumeroDocumento = NormalizarNumeroDocumento(
                    tipoDocumento.Data.Nombre,
                    datosNormalizados.NumeroDocumento)
            };

            var personaExistente = await _unidadDeTrabajo.TPersona.ObtenerEntidadAsync(
                x => x.TipoDocumentoId == datos.TipoDocumentoId && x.NumeroDocumento == datosNormalizados.NumeroDocumento);
            if (!string.IsNullOrEmpty(personaExistente.Error))
            {
                resultado.Error = "No fue posible validar el número de documento.";
                return resultado;
            }

            if (personaExistente.Data != null)
            {
                resultado.Error = "Ya existe una persona registrada con ese tipo y número de documento.";
                return resultado;
            }

            var correoExistente = await _unidadDeTrabajo.TPersona.ObtenerEntidadAsync(
                x => x.Email == datosNormalizados.Email);
            if (!string.IsNullOrEmpty(correoExistente.Error))
            {
                resultado.Error = "No fue posible validar el correo electrónico.";
                return resultado;
            }

            if (correoExistente.Data != null)
            {
                resultado.Error = "El correo electrónico ya está registrado.";
                return resultado;
            }

            var usuarioExistente = await _unidadDeTrabajo.TUsuario.ObtenerEntidadAsync(
                x => x.NombreUsuario == datosNormalizados.NombreUsuario);
            if (!string.IsNullOrEmpty(usuarioExistente.Error))
            {
                resultado.Error = "No fue posible validar el nombre de usuario.";
                return resultado;
            }

            if (usuarioExistente.Data != null)
            {
                resultado.Error = "El nombre de usuario ya está registrado.";
                return resultado;
            }

            var rolCliente = await _unidadDeTrabajo.TRol.ObtenerEntidadAsync(x => x.Nombre == "Cliente");
            if (!string.IsNullOrEmpty(rolCliente.Error))
            {
                resultado.Error = "No fue posible validar el rol de cliente.";
                return resultado;
            }

            if (rolCliente.Data == null)
            {
                resultado.Error = "No existe el rol Cliente requerido para completar el registro.";
                return resultado;
            }

            var clienteExistente = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(
                x => x.Cedula == datosNormalizados.NumeroDocumento);
            if (!string.IsNullOrEmpty(clienteExistente.Error))
            {
                resultado.Error = "No fue posible validar el documento del cliente.";
                return resultado;
            }

            if (clienteExistente.Data != null)
            {
                resultado.Error = "Ya existe un cliente registrado con ese número de documento.";
                return resultado;
            }

            var fechaRegistro = DateTime.UtcNow.Date;
            var persona = new Persona
            {
                TipoDocumentoId = tipoDocumento.Data.TipoDocumentoId,
                NumeroDocumento = datosNormalizados.NumeroDocumento,
                Nombre = datosNormalizados.Nombre,
                Apellido = datosNormalizados.Apellido,
                FechaNacimiento = datos.FechaNacimiento?.Date,
                Telefono = datosNormalizados.Telefono,
                Email = datosNormalizados.Email
            };

            persona.Usuarios.Add(new Usuario
            {
                RolId = rolCliente.Data.RolId,
                NombreUsuario = datosNormalizados.NombreUsuario,
                Contrasena = _hashContrasena.CrearHash(datos.Contrasena),
                FechaRegistro = fechaRegistro,
                Activo = true
            });

            var cliente = new Cliente
            {
                Cedula = datosNormalizados.NumeroDocumento,
                FechaRegistro = fechaRegistro
            };
            cliente.DireccionClientes.Add(new DireccionCliente
            {
                Provincia = datosNormalizados.Provincia,
                Canton = datosNormalizados.Canton,
                Distrito = datosNormalizados.Distrito,
                SenaExacta = datosNormalizados.SenaExacta,
                EsPrincipal = true
            });
            persona.Clientes.Add(cliente);

            var transaccionIniciada = false;
            try
            {
                _unidadDeTrabajo.EmpezarTransaccion();
                transaccionIniciada = true;

                var insercion = await _unidadDeTrabajo.TPersona.InsertarAsync(persona);
                if (!string.IsNullOrEmpty(insercion.Error) || insercion.Data == null)
                {
                    _unidadDeTrabajo.Rollback();
                    resultado.Error = "No fue posible completar el registro del cliente.";
                    return resultado;
                }

                _unidadDeTrabajo.CompletarTran();
                resultado.Data = true;
            }
            catch (Exception ex)
            {
                if (transaccionIniciada)
                {
                    try
                    {
                        _unidadDeTrabajo.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        _logger.LogError(rollbackEx, "Error al revertir el registro de cliente.");
                    }
                }

                _logger.LogError(ex, "Error al registrar cliente.");
                resultado.Error = "No fue posible completar el registro del cliente.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar el registro de cliente.");
            resultado.Error = "No fue posible completar el registro del cliente.";
        }

        return resultado;
    }

    private static bool TryNormalizarDatos(TRegistroCliente datos, out DatosNormalizados normalizados, out string error)
    {
        normalizados = new DatosNormalizados(
            NormalizarTexto(datos.NumeroDocumento),
            NormalizarTexto(datos.Nombre),
            NormalizarTexto(datos.Apellido),
            NormalizarTexto(datos.NombreUsuario),
            NormalizarTexto(datos.Provincia),
            NormalizarTexto(datos.Canton),
            NormalizarTexto(datos.Distrito),
            NormalizarTextoOpcional(datos.Telefono),
            NormalizarEmail(datos.Email),
            NormalizarTextoOpcional(datos.SenaExacta));

        if (string.IsNullOrWhiteSpace(normalizados.Email))
        {
            error = "El correo electrónico es obligatorio.";
            return false;
        }

        if (!new EmailAddressAttribute().IsValid(normalizados.Email))
        {
            error = "El correo electrónico no tiene un formato válido.";
            return false;
        }

        if (!EsContrasenaValida(datos.Contrasena))
        {
            error = "La contraseña debe tener entre 8 y 128 caracteres e incluir al menos una mayúscula, una minúscula y un dígito.";
            return false;
        }

        if (datos.FechaNacimiento.HasValue && datos.FechaNacimiento.Value.Date > DateTime.UtcNow.Date)
        {
            error = "La fecha de nacimiento no puede ser futura.";
            return false;
        }

        if (ContieneCaracterControl(
            normalizados.NumeroDocumento,
            normalizados.Nombre,
            normalizados.Apellido,
            normalizados.NombreUsuario,
            normalizados.Provincia,
            normalizados.Canton,
            normalizados.Distrito,
            normalizados.Telefono,
            normalizados.Email,
            normalizados.SenaExacta))
        {
            error = "Los datos no pueden contener caracteres de control.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static bool EsDocumentoValido(string nombreTipoDocumento, string numeroDocumento, out string error)
    {
        var tipoNormalizado = nombreTipoDocumento.Trim().ToUpperInvariant();
        var documentoSinSeparadores = new string(numeroDocumento.Where(x => !char.IsWhiteSpace(x) && x != '-').ToArray());

        switch (tipoNormalizado)
        {
            case "CÉDULA DE IDENTIDAD COSTARRICENSE":
            case "CEDULA DE IDENTIDAD COSTARRICENSE":
                if (documentoSinSeparadores.Length == 9 && documentoSinSeparadores.All(char.IsDigit))
                {
                    error = string.Empty;
                    return true;
                }

                error = "La cédula debe contener exactamente 9 dígitos.";
                return false;

            case "DIMEX":
                if (documentoSinSeparadores.Length == 12 && documentoSinSeparadores.All(char.IsDigit))
                {
                    error = string.Empty;
                    return true;
                }

                error = "El DIMEX debe contener exactamente 12 dígitos.";
                return false;

            case "PASAPORTE":
                if (!string.IsNullOrWhiteSpace(numeroDocumento) && numeroDocumento.Length <= 30)
                {
                    error = string.Empty;
                    return true;
                }

                error = "El pasaporte es obligatorio y no puede superar 30 caracteres.";
                return false;

            default:
                error = "El tipo de documento indicado no está admitido para el registro.";
                return false;
        }
    }

    private static string NormalizarTexto(string? valor) => valor?.Trim() ?? string.Empty;

    private static string? NormalizarTextoOpcional(string? valor) => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    private static string NormalizarEmail(string? valor) => valor?.Trim().ToLowerInvariant() ?? string.Empty;

    private static bool EsContrasenaValida(string? contrasena) =>
        contrasena is { Length: >= 8 and <= 128 } &&
        contrasena.Any(x => x is >= 'A' and <= 'Z') &&
        contrasena.Any(x => x is >= 'a' and <= 'z') &&
        contrasena.Any(x => x is >= '0' and <= '9');

    private static string NormalizarNumeroDocumento(string nombreTipoDocumento, string numeroDocumento)
    {
        var tipoNormalizado = nombreTipoDocumento.Trim().ToUpperInvariant();
        if (tipoNormalizado is "CÉDULA DE IDENTIDAD COSTARRICENSE" or "CEDULA DE IDENTIDAD COSTARRICENSE" or "DIMEX")
        {
            return new string(numeroDocumento.Where(x => !char.IsWhiteSpace(x) && x != '-').ToArray());
        }

        return numeroDocumento;
    }

    private static bool ContieneCaracterControl(params string?[] valores) => valores
        .Where(x => !string.IsNullOrEmpty(x))
        .Any(x => x!.Any(char.IsControl));

    private sealed record DatosNormalizados(
        string NumeroDocumento,
        string Nombre,
        string Apellido,
        string NombreUsuario,
        string Provincia,
        string Canton,
        string Distrito,
        string? Telefono,
        string Email,
        string? SenaExacta);
}
