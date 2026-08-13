using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN;

public interface IAccesoUsuarioLN
{
    Task<Respuesta<TSesionUsuario>> IniciarSesionAsync(TInicioSesion datos);
}
