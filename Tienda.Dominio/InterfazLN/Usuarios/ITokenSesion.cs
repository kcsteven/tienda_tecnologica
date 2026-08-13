using Tienda.Dominio.EntidadesTipadas;

namespace Tienda.Dominio.InterfazLN;

public interface ITokenSesion
{
    TSesionUsuario CrearSesion(
        int usuarioId,
        int personaId,
        string nombreUsuario,
        string nombreCompleto,
        string email,
        string rol);
}
