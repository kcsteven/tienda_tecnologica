
using System.Collections.Generic;
using System.Threading.Tasks;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN;

public interface IPagoLN
{
    Task<Respuesta<TPago>> InsertarAsync(TPago datos);

    Task<Respuesta<bool>> EliminarAsync(TPago datos);

    Task<Respuesta<IEnumerable<TPago>>> ListarPorPedidoAsync(
        int pedidoId
    );
}