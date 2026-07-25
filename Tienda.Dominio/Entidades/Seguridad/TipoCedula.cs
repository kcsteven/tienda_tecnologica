using System;
using System.Collections.Generic;

namespace Tienda.Dominio.Entidades;

public partial class TipoCedula
{
    public int TipoCedula1 { get; set; }

    public string Descripcion { get; set; } = null!;

    public virtual ICollection<Subcategoria> Clientes { get; set; } = new List<Subcategoria>();

    public virtual ICollection<SegUsuario> SegUsuarios { get; set; } = new List<SegUsuario>();
}
