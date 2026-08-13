using System.Collections.Generic;

namespace Tienda.Dominio.Entidades;
//Tipo de indentificacion permitidos durante el registro
public partial class TipoDocumento
{
    public int TipoDocumentoId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Persona> Personas { get; set; } = new List<Persona>();
}
