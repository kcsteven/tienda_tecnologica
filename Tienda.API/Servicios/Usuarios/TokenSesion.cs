using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Servicios.Usuarios;

public class TokenSesion : ITokenSesion
{
    private const string EmisorPredeterminado = "Tienda.API";
    private const string AudienciaPredeterminada = "Tienda.Angular";
    private const int DuracionPredeterminadaMinutos = 60;
    private readonly IConfiguration _configuracion;

    public TokenSesion(IConfiguration configuracion)
    {
        _configuracion = configuracion;
    }

    public TSesionUsuario CrearSesion(
        int usuarioId,
        int personaId,
        string nombreUsuario,
        string nombreCompleto,
        string email,
        string rol)
    {
        var clave = ObtenerClave();
        var emisor = ObtenerTextoConfiguracion("Jwt:Emisor", EmisorPredeterminado);
        var audiencia = ObtenerTextoConfiguracion("Jwt:Audiencia", AudienciaPredeterminada);
        var emisionUtc = DateTime.UtcNow;
        var expiraEnUtc = emisionUtc.AddMinutes(ObtenerDuracionMinutos());

        // Claims para identificar al usuario y autorizar su rol sin exponer datos
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuarioId.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.NameIdentifier, usuarioId.ToString(CultureInfo.InvariantCulture)),
            new("personaId", personaId.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.Name, nombreUsuario),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.Role, rol),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
        };

        var credenciales = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
            SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(
            emisor,
            audiencia,
            claims,
            notBefore: emisionUtc,
            expires: expiraEnUtc,
            signingCredentials: credenciales);

        return new TSesionUsuario
        {
            UsuarioId = usuarioId,
            PersonaId = personaId,
            NombreUsuario = nombreUsuario,
            NombreCompleto = nombreCompleto,
            Email = email,
            Rol = rol,
            Token = new JwtSecurityTokenHandler().WriteToken(jwt),
            ExpiraEnUtc = expiraEnUtc
        };
    }

    private string ObtenerClave()
    {
        var clave = _configuracion["Jwt:Clave"];
        if (string.IsNullOrWhiteSpace(clave) || clave.Length < 32)
        {
            throw new InvalidOperationException("La configuración Jwt:Clave es obligatoria y debe tener al menos 32 caracteres.");
        }

        return clave;
    }

    private int ObtenerDuracionMinutos()
    {
        var valorConfigurado = _configuracion["Jwt:DuracionMinutos"];
        if (string.IsNullOrWhiteSpace(valorConfigurado))
        {
            return DuracionPredeterminadaMinutos;
        }

        if (!int.TryParse(valorConfigurado, NumberStyles.None, CultureInfo.InvariantCulture, out var duracion) ||
            duracion < 5 || duracion > 1440)
        {
            throw new InvalidOperationException("La configuración Jwt:DuracionMinutos debe estar entre 5 y 1440.");
        }

        return duracion;
    }

    private string ObtenerTextoConfiguracion(string clave, string valorPredeterminado)
    {
        var valor = _configuracion[clave];
        return string.IsNullOrWhiteSpace(valor) ? valorPredeterminado : valor;
    }
}
