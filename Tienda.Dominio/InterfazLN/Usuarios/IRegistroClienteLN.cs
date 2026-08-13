using Tienda.Dominio.EntidadesTipadas;
using Tienda.Utilidades;

namespace Tienda.Dominio.InterfazLN;

public interface IRegistroClienteLN
{
    Task<Respuesta<bool>> RegistrarAsync(TRegistroCliente datos);

    Task<Respuesta<IEnumerable<TTipoDocumento>>> ListarTiposDocumentoAsync();
}
