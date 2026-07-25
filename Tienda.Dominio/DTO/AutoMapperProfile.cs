using AutoMapper;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;

namespace Tienda.Dominio.DTO
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile() { 
        
            CreateMap<TPedido, Pedido>().ReverseMap();
            CreateMap<TCategoria, Categoria>().ReverseMap();
            CreateMap<TSubcategoria, Subcategoria>().ReverseMap();
            CreateMap<TDetallePedido, DetallesPedido>().ReverseMap();
            CreateMap<TProducto, Producto>().ReverseMap();
            CreateMap<TMarca, Marca>().ReverseMap();
            CreateMap<TProveedor, Proveedor>().ReverseMap();
            CreateMap<TCliente, Cliente>().ReverseMap();
            CreateMap<TBodega, Bodega>().ReverseMap();
            CreateMap<TDescuento, Descuento>().ReverseMap();
            CreateMap<TGarantia, Garantia>().ReverseMap();
            CreateMap<TEtiqueta, Etiqueta>().ReverseMap();
            CreateMap<TInventario, Inventario>().ReverseMap();
            CreateMap<TProductoDescuento, ProductoDescuento>().ReverseMap();
            CreateMap<TImagenProducto, ImagenProducto>().ReverseMap();
            CreateMap<TProductoGarantia, ProductoGarantia>().ReverseMap();
            CreateMap<TProductoEtiqueta, ProductoEtiqueta>().ReverseMap();
            CreateMap<TPago, Pago>().ReverseMap();
            CreateMap<TEnvio, Envio>().ReverseMap();
            CreateMap<TResena, Resena>().ReverseMap();
            CreateMap<TListaDeseos, ListaDeseos>().ReverseMap();
            CreateMap<TDevolucion, Devolucion>().ReverseMap();
        }


    }
}
