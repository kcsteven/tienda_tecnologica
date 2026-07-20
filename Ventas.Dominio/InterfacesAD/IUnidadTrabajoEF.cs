using System;
using System.Collections.Generic;
using System.Text;
using Ventas.Dominio.Entidades;

namespace Ventas.Dominio.InterfacesAD
{
    public interface IUnidadTrabajoEF : IDisposable
    {
        IRepositorioAD<Categoria> TCategoria { get; }
        IRepositorioAD<Producto> TProducto { get; }
        IRepositorioAD<Subcategoria> TSubcategoria { get; }
        IRepositorioAD<Pedido> TPedido { get; }
        IRepositorioAD<DetallesPedido> TDetallePedido { get; }
        IRepositorioAD<SegPantalla> TSegPantalla { get; }
        IRepositorioAD<SegPerfil> TSegPerfil { get; }
        IRepositorioAD<SegPerfilXpantalla> TSegPerfilXpantalla { get; }
        IRepositorioAD<SegUsuario> TSegUsuario { get; }
        IRepositorioAD<TipoCedula> TTipoCedula { get; }

        IRepositorioAD<Cliente> TCliente { get; }

        IRepositorioAD<Marca> TMarca { get; }
        IRepositorioAD<Proveedor> TProveedor { get; }
        int Completar();
        void CompletarTran();
        void EmpezarTransaccion();
        void Rollback();
        void CerrarConexion();
    }
}
