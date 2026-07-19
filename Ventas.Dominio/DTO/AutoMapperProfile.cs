using AutoMapper;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Ventas.Dominio.Entidades;
using Ventas.Dominio.EntidadesTipadas;

namespace Ventas.Dominio.DTO
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile() { 
        
            CreateMap<TPedido, Pedido>().ReverseMap();
            CreateMap<TCategoria, Categoria>().ReverseMap();
            CreateMap<TSubcategoria, Subcategoria>().ReverseMap();
            CreateMap<TDetallePedido, DetallesPedido>().ReverseMap();
            CreateMap<TProducto, Producto>().ReverseMap();
        }


    }
}
