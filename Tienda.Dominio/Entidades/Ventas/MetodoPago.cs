using System;

namespace Tienda.Dominio.Entidades;

public partial class MetodoPago
{
    public int MetodoPagoId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}