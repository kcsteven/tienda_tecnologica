using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ventas.Dominio.Entidades;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Dominio.InterfacesAD;
using Ventas.Dominio.InterfazLN;
using Ventas.Utilidades;

namespace Ventas.LogicaNegocio.Implementaciones
{
    public class ProductoLN : IProductoLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }


        private ILogger<ProductoLN> _logger { get; }


        private readonly IMapper _mapper;


        public ProductoLN(

            IUnidadTrabajoEF unidadTrabajo,

            ILogger<ProductoLN> logger,

            IMapper mapper)

        {

            _unidadDeTrabajo = unidadTrabajo;

            _logger = logger;

            _mapper = mapper;

        }


        public async Task<Respuesta<TProducto>> InsertarAsync(TProducto datos)

        {

            var resultado = new Respuesta<TProducto>();


            try
            {

                var productoExistente =

                    await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(

                        x => x.Nombre == datos.Nombre);


                if (productoExistente.Data != null)

                {

                    resultado.Data =

                        _mapper.Map<TProducto>(productoExistente.Data);


                    resultado.Error =

                        "Ya existe un producto registrado con ese nombre.";


                    return resultado;

                }

                var subcategoria =
                    await _unidadDeTrabajo.TSubcategoria.ObtenerEntidadAsync(
                        x => x.SubcategoriaId == datos.SubcategoriaId);

                if (subcategoria.Data == null)
                {
                    resultado.Error = "La subcategoría indicada no existe.";
                    return resultado;
                }

                var marca =
                    await _unidadDeTrabajo.TMarca.ObtenerEntidadAsync(
                        x => x.MarcaId == datos.MarcaId);

                if (marca.Data == null)
                {
                    resultado.Error = "La marca indicada no existe.";
                    return resultado;
                }

                var proveedor =
                    await _unidadDeTrabajo.TProveedor.ObtenerEntidadAsync(
                        x => x.ProveedorId == datos.ProveedorId);

                if (proveedor.Data == null)
                {
                    resultado.Error = "El proveedor indicado no existe.";
                    return resultado;
                }


                datos.CreadoEn = DateTime.UtcNow;


                var entidad = _mapper.Map<Producto>(datos);


                var respuestaRepositorio =

                    await _unidadDeTrabajo.TProducto.InsertarAsync(entidad);


                _unidadDeTrabajo.Completar();


                resultado.Data =

                    _mapper.Map<TProducto>(respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al insertar producto {NombreProducto}",

                    datos.Nombre);


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<IEnumerable<TProducto>>> ListarAsync()

        {

            var resultado =

                new Respuesta<IEnumerable<TProducto>>();


            try
            {

                var resp =

                    await _unidadDeTrabajo.TProducto.ListarAsync();


                resultado.Data =

                    _mapper.Map<IEnumerable<TProducto>>(resp.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al listar productos.");


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<TProducto>> ModificarAsync(TProducto datos)

        {

            var resultado =

                new Respuesta<TProducto>();


            try
            {

                var productoActual =

                    await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(

                        x => x.ProductoId == datos.ProductoId);


                if (productoActual.Data == null)

                {

                    resultado.Error =

                        "No existe el producto a modificar.";


                    return resultado;

                }


                datos.ActualizadoEn = DateTime.UtcNow;


                _mapper.Map(datos, productoActual.Data);


                var respuestaRepositorio =

                    await _unidadDeTrabajo.TProducto.ModificarAsync(

                        productoActual.Data);


                _unidadDeTrabajo.Completar();


                resultado.Data =

                    _mapper.Map<TProducto>(respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al modificar ProductoId {ProductoId}",

                    datos.ProductoId);


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<bool>> EliminarAsync(TProducto datos)

        {

            var resultado =

                new Respuesta<bool>();


            try
            {

                var producto =

                    await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(

                        x => x.ProductoId == datos.ProductoId);


                if (producto.Data == null)

                {

                    resultado.Error =

                        "No existe el producto a eliminar.";


                    return resultado;

                }


                var respuestaRepositorio =

                    await _unidadDeTrabajo.TProducto.EliminarAsync(

                        producto.Data);


                _unidadDeTrabajo.Completar();


                resultado.Data =

                    respuestaRepositorio.Data;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al eliminar ProductoId {ProductoId}",

                    datos.ProductoId);


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<IEnumerable<TProducto>>> BuscarAsync(

            TProducto datos)

        {

            var resultado =

                new Respuesta<IEnumerable<TProducto>>();


            try
            {

                var respuestaRepositorio =

                    await _unidadDeTrabajo.TProducto.BuscarAsync(

                        x => x.Nombre.Contains(

                            datos.Nombre));


                resultado.Data =

                    _mapper.Map<IEnumerable<TProducto>>(

                        respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al buscar productos.");


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        public async Task<Respuesta<TProducto>> ObtenerAsync(

            TProducto datos)

        {

            var resultado =

                new Respuesta<TProducto>();


            try
            {

                var respuestaRepositorio =

                    await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(

                        x => x.ProductoId == datos.ProductoId);


                if (respuestaRepositorio.Data == null)

                {

                    resultado.Error =

                        "Producto no encontrado.";


                    return resultado;

                }


                resultado.Data =

                    _mapper.Map<TProducto>(

                        respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex,

                    "Error al obtener ProductoId {ProductoId}",

                    datos.ProductoId);


                resultado.Error = ex.Message;

            }


            return resultado;

        }

    }

}