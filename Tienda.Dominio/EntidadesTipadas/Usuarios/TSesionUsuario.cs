namespace Tienda.Dominio.EntidadesTipadas;
//Contiene los datos seguros de la sesion
public class TSesionUsuario
{
    public int UsuarioId { get; set; }

    public int PersonaId { get; set; }

    // Solo tiene valor cuando el Rol es "Cliente"; se usa para armar pedidos/compras
    public int? ClienteId { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public DateTime ExpiraEnUtc { get; set; }
}