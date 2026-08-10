using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

    // Catálogo local usado para impedir combinaciones provincia-cantón manipuladas.
    private static readonly IReadOnlyDictionary<string, string[]> CatalogoProvinciasCantones =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["San José"] = new[] { "San José", "Escazú", "Desamparados", "Puriscal", "Tarrazú", "Aserrí", "Mora", "Goicoechea", "Santa Ana", "Alajuelita", "Vázquez de Coronado", "Acosta", "Tibás", "Moravia", "Montes de Oca", "Turrubares", "Dota", "Curridabat", "Pérez Zeledón", "León Cortés Castro" },
            ["Alajuela"] = new[] { "Alajuela", "San Ramón", "Grecia", "San Mateo", "Atenas", "Naranjo", "Palmares", "Poás", "Orotina", "San Carlos", "Zarcero", "Sarchí", "Upala", "Los Chiles", "Guatuso", "Río Cuarto" },
            ["Cartago"] = new[] { "Cartago", "Paraíso", "La Unión", "Jiménez", "Turrialba", "Alvarado", "Oreamuno", "El Guarco" },
            ["Heredia"] = new[] { "Heredia", "Barva", "Santo Domingo", "Santa Bárbara", "San Rafael", "San Isidro", "Belén", "Flores", "San Pablo", "Sarapiquí" },
            ["Guanacaste"] = new[] { "Liberia", "Nicoya", "Santa Cruz", "Bagaces", "Carrillo", "Cañas", "Abangares", "Tilarán", "Nandayure", "La Cruz", "Hojancha" },
            ["Puntarenas"] = new[] { "Puntarenas", "Esparza", "Buenos Aires", "Montes de Oro", "Osa", "Quepos", "Golfito", "Coto Brus", "Parrita", "Corredores", "Garabito", "Monteverde", "Puerto Jiménez" },
            ["Limón"] = new[] { "Limón", "Pococí", "Siquirres", "Talamanca", "Matina", "Guácimo" }
        };

    private static readonly Regex CorreoGmailRegex = new(
        "^[a-z0-9._+\\-]+@gmail\\.com$",
        RegexOptions.CultureInvariant);

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
            datos.NumeroDocumento ?? string.Empty,
            NormalizarTexto(datos.Nombre),
            NormalizarTexto(datos.Apellido),
            NormalizarTexto(datos.NombreUsuario),
            NormalizarTexto(datos.Provincia),
            NormalizarTexto(datos.Canton),
            NormalizarTexto(datos.Distrito),
            NormalizarTextoOpcional(datos.Telefono),
            NormalizarEmail(datos.Email),
            NormalizarTextoOpcional(datos.SenaExacta));

        if (string.IsNullOrWhiteSpace(normalizados.Nombre))
        {
            error = "El nombre es obligatorio.";
            return false;
        }

        if (!EsTextoSoloLetrasYEspacios(normalizados.Nombre))
        {
            error = "El nombre solo puede contener letras";
            return false;
        }

        if (string.IsNullOrWhiteSpace(normalizados.Apellido))
        {
            error = "El apellido es obligatorio.";
            return false;
        }

        if (!EsTextoSoloLetrasYEspacios(normalizados.Apellido))
        {
            error = "El apellido solo puede contener letras";
            return false;
        }

        if (string.IsNullOrWhiteSpace(normalizados.Distrito))
        {
            error = "El distrito es obligatorio.";
            return false;
        }

        if (!EsTextoSoloLetrasYEspacios(normalizados.Distrito))
        {
            error = "El distrito solo puede contener letras";
            return false;
        }

        if (string.IsNullOrWhiteSpace(normalizados.Email))
        {
            error = "El correo electrónico es obligatorio.";
            return false;
        }

        // El registro público admite únicamente direcciones Gmail con formato controlado.
        if (!CorreoGmailRegex.IsMatch(normalizados.Email))
        {
            error = "Ingrese un correo valido";
            return false;
        }

        var provinciaRecibida = normalizados.Provincia;
        var cantonRecibido = normalizados.Canton;
        if (string.IsNullOrWhiteSpace(provinciaRecibida))
        {
            error = "La provincia es obligatoria.";
            return false;
        }

        if (!CatalogoProvinciasCantones.TryGetValue(provinciaRecibida, out var cantonesProvincia))
        {
            error = "Seleccione una provincia válida.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(cantonRecibido))
        {
            error = "El cantón es obligatorio.";
            return false;
        }

        var provinciaCanonica = CatalogoProvinciasCantones.Keys.First(x =>
            string.Equals(x, provinciaRecibida, StringComparison.OrdinalIgnoreCase));
        var cantonCanonico = cantonesProvincia.FirstOrDefault(x =>
            string.Equals(x, cantonRecibido, StringComparison.OrdinalIgnoreCase));
        if (cantonCanonico is null)
        {
            error = "Seleccione un cantón válido para la provincia.";
            return false;
        }

        normalizados = normalizados with
        {
            Provincia = provinciaCanonica,
            Canton = cantonCanonico
        };

        // La fecha se valida contra la fecha local del servidor, no contra el cliente.
        if (!datos.FechaNacimiento.HasValue || !EsMayorDeEdad(datos.FechaNacimiento.Value.Date))
        {
            error = "Necesitas ser mayor de 18 años";
            return false;
        }

        if (!EsContrasenaValida(datos.Contrasena))
        {
            error = "La contraseña debe tener entre 8 y 128 caracteres e incluir al menos una mayúscula, una minúscula y un dígito.";
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
            error = "Los datos no pueden contener caracteres especiales.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private static bool EsDocumentoValido(string nombreTipoDocumento, string numeroDocumento, out string error)
    {
        var tipoNormalizado = nombreTipoDocumento.Trim().ToUpperInvariant();

        // Las reglas se determinan por el nombre del tipo, no por un identificador fijo.
        switch (tipoNormalizado)
        {
            case "CÉDULA DE IDENTIDAD COSTARRICENSE":
            case "CEDULA DE IDENTIDAD COSTARRICENSE":
                if (!numeroDocumento.All(char.IsDigit))
                {
                    error = "El numero de documento solo puede contener numeros";
                    return false;
                }

                if (numeroDocumento.Length == 9)
                {
                    error = string.Empty;
                    return true;
                }

                error = "La cédula debe contener exactamente 9 dígitos.";
                return false;

            case "DIMEX":
                if (!numeroDocumento.All(char.IsDigit))
                {
                    error = "El numero de documento solo puede contener numeros";
                    return false;
                }

                if (numeroDocumento.Length is 11 or 12)
                {
                    error = string.Empty;
                    return true;
                }

                error = "El DIMEX debe contener 11 o 12 dígitos.";
                return false;

            case "PASAPORTE":
                var pasaporteNormalizado = numeroDocumento.Trim().ToUpperInvariant();
                if (!pasaporteNormalizado.All(x => x is >= 'A' and <= 'Z' or >= '0' and <= '9'))
                {
                    error = "El pasaporte solo puede contener letras y números.";
                    return false;
                }

                if (pasaporteNormalizado.Length is >= 5 and <= 20)
                {
                    error = string.Empty;
                    return true;
                }

                error = "El pasaporte debe contener entre 5 y 20 caracteres.";
                return false;

            default:
                error = "El tipo de documento indicado no está admitido para el registro.";
                return false;
        }
    }

    private static string NormalizarTexto(string? valor) => valor?.Trim() ?? string.Empty;

    private static string? NormalizarTextoOpcional(string? valor) => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    private static string NormalizarEmail(string? valor) => valor?.Trim().ToLowerInvariant() ?? string.Empty;

    private static bool EsTextoSoloLetrasYEspacios(string valor) =>
        valor.All(x => char.IsLetter(x) || x == ' ');

    // Calcula la edad por año, mes y día para permitir el registro al cumplir 18 años.
    private static bool EsMayorDeEdad(DateTime fechaNacimiento)
    {
        var hoy = DateTime.Today;
        if (fechaNacimiento > hoy)
        {
            return false;
        }

        var edad = hoy.Year - fechaNacimiento.Year;
        if (fechaNacimiento > hoy.AddYears(-edad))
        {
            edad--;
        }

        return edad >= 18;
    }

    private static bool EsContrasenaValida(string? contrasena) =>
        contrasena is { Length: >= 8 and <= 128 } &&
        contrasena.Any(x => x is >= 'A' and <= 'Z') &&
        contrasena.Any(x => x is >= 'a' and <= 'z') &&
        contrasena.Any(x => x is >= '0' and <= '9');

    private static string NormalizarNumeroDocumento(string nombreTipoDocumento, string numeroDocumento)
    {
        var tipoNormalizado = nombreTipoDocumento.Trim().ToUpperInvariant();
        if (tipoNormalizado == "PASAPORTE")
        {
            return numeroDocumento.Trim().ToUpperInvariant();
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
