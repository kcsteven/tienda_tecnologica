using System;

namespace Tienda.Dominio.Entidades;
//Muestra credenciales, estado y el rol usado para ingresar al sistema
public partial class Usuario
{
    public int UsuarioId { get; set; }

    public int PersonaId { get; set; }

    public int RolId { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public bool Activo { get; set; }

    public virtual Persona Persona { get; set; } = null!;

    public virtual Rol Rol { get; set; } = null!;
}
