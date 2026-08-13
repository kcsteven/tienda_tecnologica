using System.Collections.Generic;

namespace Tienda.Dominio.Entidades;
//Define que nivel de acceso tiene el usuario
public partial class Rol
{
    public int RolId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
