using System;
using System.Collections.Generic;

namespace Tienda.Dominio.Entidades;

public partial class Persona
{
    public int PersonaId { get; set; }

    public int TipoDocumentoId { get; set; }

    public string NumeroDocumento { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public DateTime? FechaNacimiento { get; set; }

    public string? Telefono { get; set; }

    public string? Email { get; set; }

    public virtual TipoDocumento TipoDocumento { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
