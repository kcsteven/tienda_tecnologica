using System;
using System.Collections.Generic;
using System.Text;
using Tienda.Dominio.Entidades;

namespace Tienda.Dominio.InterfacesAD
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
        IRepositorioAD<Persona> TPersona { get; }
        IRepositorioAD<TipoDocumento> TTipoDocumento { get; }
        IRepositorioAD<Rol> TRol { get; }
        IRepositorioAD<Usuario> TUsuario { get; }
        IRepositorioAD<Marca> TMarca { get; }
        IRepositorioAD<Proveedor> TProveedor { get; }
        IRepositorioAD<Bodega> TBodega { get; }
        IRepositorioAD<Descuento> TDescuento { get; }
        IRepositorioAD<Garantia> TGarantia { get; }
        IRepositorioAD<Etiqueta> TEtiqueta { get; }

        IRepositorioAD<Inventario> TInventario { get; }
        IRepositorioAD<ProductoDescuento> TProductoDescuento { get; }
        IRepositorioAD<ImagenProducto> TImagenProducto { get; }
        IRepositorioAD<ProductoGarantia> TProductoGarantia { get; }
        IRepositorioAD<ProductoEtiqueta> TProductoEtiqueta { get; }
        IRepositorioAD<Pago> TPago { get; }
        IRepositorioAD<Envio> TEnvio { get; }
        IRepositorioAD<Resena> TResena { get; }
        IRepositorioAD<ListaDeseos> TListaDeseos { get; }
        IRepositorioAD<Devolucion> TDevolucion { get; }
        int Completar();
        void CompletarTran();
        void EmpezarTransaccion();
        void Rollback();
        void CerrarConexion();
    }
}
