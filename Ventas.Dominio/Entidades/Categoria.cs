using System;
using System.Collections.Generic;

namespace Ventas.Dominio.Entidades;

public partial class Categoria
{
    public int CategoriaId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }
    public DateTime CreadoEn { get; set; }
    public string? CreadoPor { get; set; }
    public DateTime? ActualizadoEn { get; set; }
    public string? ActualizadoPor { get; set; }
    public byte[] RowVer { get; set; } = null!;

    public virtual ICollection<Subcategoria> Subcategorias { get; set; } = new List<Subcategoria>();
   
}