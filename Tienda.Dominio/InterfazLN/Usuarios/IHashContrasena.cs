namespace Tienda.Dominio.InterfazLN;

public interface IHashContrasena
{
    string CrearHash(string contrasena);

    bool VerificarHash(string contrasena, string hashAlmacenado);
}
