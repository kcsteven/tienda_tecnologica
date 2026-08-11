using System.ComponentModel.DataAnnotations;

namespace Tienda.Dominio.EntidadesTipadas;

public class TInicioSesion
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(128)]
    public string Contrasena { get; set; } = string.Empty;
}
