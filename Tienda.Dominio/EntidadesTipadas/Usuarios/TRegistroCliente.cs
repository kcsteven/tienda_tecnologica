using System;
using System.ComponentModel.DataAnnotations;

namespace Tienda.Dominio.EntidadesTipadas;

public class TRegistroCliente
{
    [Range(1, int.MaxValue)]
    public int TipoDocumentoId { get; set; }

    [Required, StringLength(30)]
    public string NumeroDocumento { get; set; } = null!;

    [Required, StringLength(100)]
    public string Nombre { get; set; } = null!;

    [Required, StringLength(100)]
    public string Apellido { get; set; } = null!;

    public DateTime? FechaNacimiento { get; set; }

    [StringLength(20)]
    public string? Telefono { get; set; }

    [Required, EmailAddress, StringLength(100)]
    public string Email { get; set; } = null!;

    [Required, StringLength(50)]
    public string NombreUsuario { get; set; } = null!;

    [Required, MinLength(8), StringLength(128)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])[\s\S]+$",
        ErrorMessage = "La contraseña debe incluir al menos una mayúscula, una minúscula y un dígito.")]
    public string Contrasena { get; set; } = null!;

    [Required, StringLength(100)]
    public string Provincia { get; set; } = null!;

    [Required, StringLength(100)]
    public string Canton { get; set; } = null!;

    [Required, StringLength(100)]
    public string Distrito { get; set; } = null!;

    [StringLength(255)]
    public string? SenaExacta { get; set; }
}
