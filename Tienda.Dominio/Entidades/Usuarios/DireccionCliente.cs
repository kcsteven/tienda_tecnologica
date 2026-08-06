namespace Tienda.Dominio.Entidades;

public partial class DireccionCliente
{
    public int DireccionId { get; set; }

    public int ClienteId { get; set; }

    public string Provincia { get; set; } = null!;

    public string Canton { get; set; } = null!;

    public string Distrito { get; set; } = null!;

    public string? SenaExacta { get; set; }

    public bool EsPrincipal { get; set; }

    public virtual Cliente Cliente { get; set; } = null!;
}
