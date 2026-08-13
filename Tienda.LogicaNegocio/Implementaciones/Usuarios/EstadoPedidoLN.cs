using Microsoft.Extensions.Logging;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.LogicaNegocio.Implementaciones
{
    public class EstadoPedidoLN : IEstadoPedidoLN
    {
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;
        private readonly ILogger<EstadoPedidoLN> _logger;

        public EstadoPedidoLN(
            IUnidadTrabajoEF unidadDeTrabajo,
            ILogger<EstadoPedidoLN> logger)
        {
            _unidadDeTrabajo = unidadDeTrabajo;
            _logger = logger;
        }

        public async Task<Respuesta<IEnumerable<TEstadoPedido>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TEstadoPedido>>();

            try
            {
                var respuesta = await _unidadDeTrabajo.TEstadoPedido.BuscarAsync(x => true);

                resultado.Data = respuesta.Data?.Select(x => new TEstadoPedido
                {
                    EstadoPedidoId = x.EstadoPedidoId,
                    Nombre = x.Nombre
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar los estados de pedido.");
                resultado.Error = "Ocurrió un error al listar los estados de pedido.";
            }

            return resultado;
        }

        public async Task<Respuesta<TEstadoPedido>> ObtenerPorIdAsync(int id)
        {
            var resultado = new Respuesta<TEstadoPedido>();

            try
            {
                var respuesta = await _unidadDeTrabajo.TEstadoPedido
                    .ObtenerEntidadAsync(x => x.EstadoPedidoId == id);

                if (respuesta.Data != null)
                {
                    resultado.Data = new TEstadoPedido
                    {
                        EstadoPedidoId = respuesta.Data.EstadoPedidoId,
                        Nombre = respuesta.Data.Nombre
                    };
                }
                else
                {
                    resultado.Error = "No se encontró el estado de pedido.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el estado de pedido con ID {Id}", id);
                resultado.Error = "Ocurrió un error al obtener el estado de pedido.";
            }

            return resultado;
        }
    }
}