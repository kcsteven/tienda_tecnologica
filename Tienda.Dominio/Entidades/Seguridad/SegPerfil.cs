using System;
using System.Collections.Generic;

namespace Tienda.Dominio.Entidades;

public partial class SegPerfil
{
    public int CodigoPerfil { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<SegPerfilXpantalla> SegPerfilXpantallas { get; set; } = new List<SegPerfilXpantalla>();

    public virtual ICollection<SegUsuario> SegUsuarios { get; set; } = new List<SegUsuario>();
}
