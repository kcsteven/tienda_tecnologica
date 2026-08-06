using System;
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
}
