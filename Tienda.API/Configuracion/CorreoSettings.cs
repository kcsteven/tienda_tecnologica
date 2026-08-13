namespace Tienda.API.Configuracion;

public class CorreoSettings
{
    public string Servidor { get; set; } = null!;

    public int Puerto { get; set; }

    public string Usuario { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public string Remitente { get; set; } = null!;

    public string NombreRemitente { get; set; } = null!;
}