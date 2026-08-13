using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN
{
    public interface IEstadoPedidoLN
    {
        Task<Respuesta<IEnumerable<TEstadoPedido>>> ListarAsync();
        Task<Respuesta<TEstadoPedido>> ObtenerPorIdAsync(int id);
    }
}