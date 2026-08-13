using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Tienda.AccesoDatos.Contexto;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;

namespace Tienda.AccesoDatos.Implementaciones
{
    public class UnidadTrabajoEF : IUnidadTrabajoEF
    {
        #region "Atributos y Variables"

        private VentasContext _Contexto { get; set; }

        private IConfiguration _configuration { get; set; }

        public UnidadTrabajoEF(VentasContext Contexto, IConfiguration configuration)
        {
            _Contexto = Contexto;
            _configuration = configuration;
        }

        private IDbContextTransaction _transaction = null;

        private RepositorioAD<Categoria> _TCategoria;
        private RepositorioAD<Subcategoria> _TSubcategoria;
        private RepositorioAD<DetallesPedido> _TDetallePedido;
        private RepositorioAD<Pedido> _TPedido;
        private RepositorioAD<Producto> _TProducto;

        private RepositorioAD<SegPantalla> _TSegPantalla;
        private RepositorioAD<SegPerfil> _TSegPerfil;
        private RepositorioAD<SegPerfilXpantalla> _TSegPerfilXpantalla;
        private RepositorioAD<SegUsuario> _TSegUsuario;
        private RepositorioAD<TipoCedula> _TTipoCedula;
        private RepositorioAD<Marca> _TMarca;
        private RepositorioAD<Proveedor> _TProveedor;
        private RepositorioAD<Cliente> _TCliente;
        private IClienteAdministracionAD? _clienteAdministracionAD;
        private RepositorioAD<Persona> _TPersona;
        private RepositorioAD<TipoDocumento> _TTipoDocumento;
        private RepositorioAD<Rol> _TRol;
        private RepositorioAD<Usuario> _TUsuario;
        private RepositorioAD<Bodega> _TBodega;
        private RepositorioAD<Descuento> _TDescuento;
        private RepositorioAD<Garantia> _TGarantia;
        private RepositorioAD<Etiqueta> _TEtiqueta;
        private RepositorioAD<Inventario> _TInventario;
        private RepositorioAD<ProductoDescuento> _TProductoDescuento;
        private RepositorioAD<ImagenProducto> _TImagenProducto;
        private RepositorioAD<ProductoGarantia> _TProductoGarantia;
        private RepositorioAD<ProductoEtiqueta> _TProductoEtiqueta;

        private RepositorioAD<Pago> _TPago;
        private RepositorioAD<Envio> _TEnvio;
        private RepositorioAD<Resena> _TResena;
        private RepositorioAD<ListaDeseos> _TListaDeseos;
        private RepositorioAD<Devolucion> _TDevolucion;
        private RepositorioAD<EspecificacionProducto> _TEspecificacionProducto;

        #endregion
        #region "Constructores"

        public IRepositorioAD<Categoria> TCategoria
        {
            get
            {
                if (this._TCategoria == null)
                {
                    this._TCategoria = new RepositorioAD<Categoria>(_Contexto);
                }
                return _TCategoria;
            }
        }

        public IRepositorioAD<Subcategoria> TSubcategoria
        {
            get
            {
                if (this._TSubcategoria == null)
                {
                    this._TSubcategoria = new RepositorioAD<Subcategoria>(_Contexto);
                }
                return _TSubcategoria;
            }
        }

        public IRepositorioAD<Cliente> TCliente
        {
            get
            {
                if (this._TCliente == null)
                    this._TCliente = new RepositorioAD<Cliente>(_Contexto);
                return _TCliente;
            }
        }

        public IClienteAdministracionAD ClienteAdministracion
        {
            get
            {
                _clienteAdministracionAD ??= new ClienteAdministracionAD(_Contexto);
                return _clienteAdministracionAD;
            }
        }

        public IRepositorioAD<Persona> TPersona
        {
            get
            {
                if (this._TPersona == null)
                    this._TPersona = new RepositorioAD<Persona>(_Contexto);
                return _TPersona;
            }
        }

        public IRepositorioAD<TipoDocumento> TTipoDocumento
        {
            get
            {
                if (this._TTipoDocumento == null)
                    this._TTipoDocumento = new RepositorioAD<TipoDocumento>(_Contexto);
                return _TTipoDocumento;
            }
        }

        public IRepositorioAD<Rol> TRol
        {
            get
            {
                if (this._TRol == null)
                    this._TRol = new RepositorioAD<Rol>(_Contexto);
                return _TRol;
            }
        }

        public IRepositorioAD<Usuario> TUsuario
        {
            get
            {
                if (this._TUsuario == null)
                    this._TUsuario = new RepositorioAD<Usuario>(_Contexto);
                return _TUsuario;
            }
        }

        public IRepositorioAD<DetallesPedido> TDetallePedido
        {
            get
            {
                if (this._TDetallePedido == null)
                {
                    this._TDetallePedido = new RepositorioAD<DetallesPedido>(_Contexto);
                }
                return _TDetallePedido;
            }
        }

        public IRepositorioAD<Pedido> TPedido
        {
            get
            {
                if (this._TPedido == null)
                {
                    this._TPedido = new RepositorioAD<Pedido>(_Contexto);
                }
                return _TPedido;
            }
        }

        public IRepositorioAD<Producto> TProducto
        {
            get
            {
                if (this._TProducto == null)
                {
                    this._TProducto = new RepositorioAD<Producto>(_Contexto);
                }
                return _TProducto;
            }
        }

        public IRepositorioAD<SegPantalla> TSegPantalla
        {
            get
            {
                if (this._TSegPantalla == null)
                {
                    this._TSegPantalla = new RepositorioAD<SegPantalla>(_Contexto);
                }
                return _TSegPantalla;
            }
        }

        public IRepositorioAD<SegPerfil> TSegPerfil
        {
            get
            {
                if (this._TSegPerfil == null)
                {
                    this._TSegPerfil = new RepositorioAD<SegPerfil>(_Contexto);
                }
                return _TSegPerfil;
            }
        }

        public IRepositorioAD<SegPerfilXpantalla> TSegPerfilXpantalla
        {
            get
            {
                if (this._TSegPerfilXpantalla == null)
                {
                    this._TSegPerfilXpantalla = new RepositorioAD<SegPerfilXpantalla>(_Contexto);
                }
                return _TSegPerfilXpantalla;
            }
        }

        public IRepositorioAD<SegUsuario> TSegUsuario
        {
            get
            {
                if (this._TSegUsuario == null)
                {
                    this._TSegUsuario = new RepositorioAD<SegUsuario>(_Contexto);
                }
                return _TSegUsuario;
            }
        }

        public IRepositorioAD<TipoCedula> TTipoCedula
        {
            get
            {
                if (this._TTipoCedula == null)
                {
                    this._TTipoCedula = new RepositorioAD<TipoCedula>(_Contexto);
                }
                return _TTipoCedula;
            }
        }

        public IRepositorioAD<Marca> TMarca
        {
            get
            {
                if (this._TMarca == null)
                    this._TMarca = new RepositorioAD<Marca>(_Contexto);
                return _TMarca;
            }
        }

        public IRepositorioAD<Proveedor> TProveedor
        {
            get
            {
                if (this._TProveedor == null)
                    this._TProveedor = new RepositorioAD<Proveedor>(_Contexto);
                return _TProveedor;
            }
        }

        public IRepositorioAD<Bodega> TBodega
        {
            get
            {
                if (this._TBodega == null)
                    this._TBodega = new RepositorioAD<Bodega>(_Contexto);
                return _TBodega;
            }
        }

        public IRepositorioAD<Descuento> TDescuento
        {
            get
            {
                if (this._TDescuento == null)
                    this._TDescuento = new RepositorioAD<Descuento>(_Contexto);
                return _TDescuento;
            }
        }

        public IRepositorioAD<Garantia> TGarantia
        {
            get
            {
                if (this._TGarantia == null)
                    this._TGarantia = new RepositorioAD<Garantia>(_Contexto);
                return _TGarantia;
            }
        }

        public IRepositorioAD<EspecificacionProducto> TEspecificacionProducto
        {
            get { if (this._TEspecificacionProducto == null) this._TEspecificacionProducto = new RepositorioAD<EspecificacionProducto>(_Contexto); return _TEspecificacionProducto; }
        }

        public IRepositorioAD<Etiqueta> TEtiqueta
        {
            get
            {
                if (this._TEtiqueta == null)
                    this._TEtiqueta = new RepositorioAD<Etiqueta>(_Contexto);
                return _TEtiqueta;
            }
        }

        public IRepositorioAD<Inventario> TInventario
        {
            get { if (this._TInventario == null) this._TInventario = new RepositorioAD<Inventario>(_Contexto); return _TInventario; }
        }

        public IRepositorioAD<ProductoDescuento> TProductoDescuento
        {
            get { if (this._TProductoDescuento == null) this._TProductoDescuento = new RepositorioAD<ProductoDescuento>(_Contexto); return _TProductoDescuento; }
        }

        public IRepositorioAD<ImagenProducto> TImagenProducto
        {
            get { if (this._TImagenProducto == null) this._TImagenProducto = new RepositorioAD<ImagenProducto>(_Contexto); return _TImagenProducto; }
        }

        public IRepositorioAD<ProductoGarantia> TProductoGarantia
        {
            get { if (this._TProductoGarantia == null) this._TProductoGarantia = new RepositorioAD<ProductoGarantia>(_Contexto); return _TProductoGarantia; }
        }

        public IRepositorioAD<ProductoEtiqueta> TProductoEtiqueta
        {
            get { if (this._TProductoEtiqueta == null) this._TProductoEtiqueta = new RepositorioAD<ProductoEtiqueta>(_Contexto); return _TProductoEtiqueta; }
        }

        public IRepositorioAD<Pago> TPago
        {
            get { if (this._TPago == null) this._TPago = new RepositorioAD<Pago>(_Contexto); return _TPago; }
        }
        public IRepositorioAD<Envio> TEnvio
        {
            get { if (this._TEnvio == null) this._TEnvio = new RepositorioAD<Envio>(_Contexto); return _TEnvio; }
        }
        public IRepositorioAD<Resena> TResena
        {
            get { if (this._TResena == null) this._TResena = new RepositorioAD<Resena>(_Contexto); return _TResena; }
        }
        public IRepositorioAD<ListaDeseos> TListaDeseos
        {
            get { if (this._TListaDeseos == null) this._TListaDeseos = new RepositorioAD<ListaDeseos>(_Contexto); return _TListaDeseos; }
        }
        public IRepositorioAD<Devolucion> TDevolucion
        {
            get { if (this._TDevolucion == null) this._TDevolucion = new RepositorioAD<Devolucion>(_Contexto); return _TDevolucion; }
        }

        public int Completar()
        {
            try
            {
                return _Contexto.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void CompletarTran()
        {
            try
            {
                _Contexto.SaveChanges();
                _transaction.Commit();
            }
            catch (Exception ex)
            {
                _transaction.Rollback();
                throw ex;
            }
        }

        public void EmpezarTransaccion()
        {
            _transaction = _Contexto.Database.BeginTransaction();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void CerrarConexion()
        {
            _Contexto.Database.CloseConnection();
        }

        public void Dispose()
        {
            _Contexto.Dispose();
        }

        #endregion
    }
}
