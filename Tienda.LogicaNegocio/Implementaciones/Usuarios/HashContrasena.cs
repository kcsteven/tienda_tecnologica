using System;
using System.Globalization;
using System.Security.Cryptography;
using Tienda.Dominio.InterfazLN;

namespace Tienda.LogicaNegocio.Implementaciones;


public class HashContrasena : IHashContrasena
{
    private const string Version = "v1";
    private const string Algoritmo = "pbkdf2-sha256";
    private const int Iteraciones = 600000;
    private const int TamanoSal = 16;
    private const int TamanoHash = 32;

    //crea una sal aleatoria y crea un hash para almacenar la contraseña
    public string CrearHash(string contrasena)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contrasena);

        byte[] sal = RandomNumberGenerator.GetBytes(TamanoSal);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            contrasena,
            sal,
            Iteraciones,
            HashAlgorithmName.SHA256,
            TamanoHash);

        return string.Join(
            '$',
            Version,
            Algoritmo,
            Iteraciones,
            Convert.ToBase64String(sal),
            Convert.ToBase64String(hash));
    }

    //Compara el hash con el valor almacenado
    public bool VerificarHash(string contrasena, string hashAlmacenado)
    {
        if (string.IsNullOrWhiteSpace(contrasena) || string.IsNullOrWhiteSpace(hashAlmacenado))
        {
            return false;
        }

        try
        {
            var partes = hashAlmacenado.Split('$');
            if (partes.Length != 5 ||
                !string.Equals(partes[0], Version, StringComparison.Ordinal) ||
                !string.Equals(partes[1], Algoritmo, StringComparison.Ordinal) ||
                !int.TryParse(partes[2], NumberStyles.None, CultureInfo.InvariantCulture, out var iteraciones) ||
                iteraciones != Iteraciones)
            {
                return false;
            }

            var sal = Convert.FromBase64String(partes[3]);
            var hashEsperado = Convert.FromBase64String(partes[4]);
            if (sal.Length != TamanoSal || hashEsperado.Length != TamanoHash)
            {
                return false;
            }

            var hashCalculado = Rfc2898DeriveBytes.Pbkdf2(
                contrasena,
                sal,
                iteraciones,
                HashAlgorithmName.SHA256,
                TamanoHash);

            return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
        }
        catch (FormatException)
        {
            return false;
        }
        catch (CryptographicException)
        {
            return false;
        }
    }
}
