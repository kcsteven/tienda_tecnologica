using System.Collections.Generic;
using Tienda.Dominio.EntidadesTipadas;

namespace Tienda.API.Servicios.Correo;

public interface ICorreoService
{
    Task EnviarFacturaAsync(
        string correoDestino,
        string nombreCliente,
        int pedidoId,
        decimal subtotal,
        decimal iva,
        decimal total,
        List<TItemFactura> items);
}