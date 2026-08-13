using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN;

public interface IMetodoPagoLN
{
    Task<Respuesta<IEnumerable<TMetodoPago>>> ListarAsync();
}