using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.LogicaNegocio.Implementaciones
{
    // Implementación de la lógica de negocio (LN) para la entidad Producto
    public class ProductoLN : IProductoLN
    {
        // Unidad de trabajo (Entity Framework) para acceder a los repositorios de datos
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }


        // Logger para registrar errores y eventos de esta clase
        private ILogger<ProductoLN> _logger { get; }


        // AutoMapper para convertir entre entidades de dominio (Producto) y entidades tipadas (TProducto)
        private readonly IMapper _mapper;


        // Constructor: recibe las dependencias mediante inyección de dependencias
        public ProductoLN(

            IUnidadTrabajoEF unidadTrabajo,

            ILogger<ProductoLN> logger,

            IMapper mapper)

        {

            _unidadDeTrabajo = unidadTrabajo;

            _logger = logger;

            _mapper = mapper;

        }


        // Inserta un nuevo producto, validando que no exista ya uno con el mismo nombre
        // y que la subcategoría, marca y proveedor indicados realmente existan
        public async Task<Respuesta<TProducto>> InsertarAsync(TProducto datos)

        {

            var resultado = new Respuesta<TProducto>();


            try
            {

                // Verifica si ya existe un producto registrado con el mismo nombre
                var productoExistente =

                    await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(

                        x => x.Nombre == datos.Nombre);


                if (productoExistente.Data != null)

                {

                    // Nota: a diferencia de otras validaciones de duplicado, aquí sí se devuelve
                    // el producto existente en Data, además del mensaje de error
                    resultado.Data =

                        _mapper.Map<TProducto>(productoExistente.Data);


                    resultado.Error =

                        "Ya existe un producto registrado con ese nombre.";


                    return resultado;

                }

                // Verifica que la subcategoría indicada exista
                var subcategoria =
                    await _unidadDeTrabajo.TSubcategoria.ObtenerEntidadAsync(
                        x => x.SubcategoriaId == datos.SubcategoriaId);

                if (subcategoria.Data == null)
                {
                    resultado.Error = "La subcategoría indicada no existe.";
                    return resultado;
                }

                // Verifica que la marca indicada exista
                var marca =
                    await _unidadDeTrabajo.TMarca.ObtenerEntidadAsync(
                        x => x.MarcaId == datos.MarcaId);

                if (marca.Data == null)
                {
                    resultado.Error = "La marca indicada no existe.";
                    return resultado;
                }

                // Verifica que el proveedor indicado exista
                var proveedor =
                    await _unidadDeTrabajo.TProveedor.ObtenerEntidadAsync(
                        x => x.ProveedorId == datos.ProveedorId);

                if (proveedor.Data == null)
                {
                    resultado.Error = "El proveedor indicado no existe.";
                    return resultado;
                }





                // Convierte el DTO tipado a la entidad de dominio
                var entidad = _mapper.Map<Producto>(datos);


                // Inserta la entidad en el repositorio
                var respuestaRepositorio =

                    await _unidadDeTrabajo.TProducto.InsertarAsync(entidad);


                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();


                // Convierte la entidad insertada de vuelta a DTO tipado para la respuesta
                resultado.Data =

                    _mapper.Map<TProducto>(respuestaRepositorio.Data);

            }

            catch (Exception ex)

            {

                // Registra el error y lo devuelve en la respuesta
                _logger.LogError(ex,

                    "Error al insertar producto {NombreProducto}",

                    datos.Nombre);


                resultado.Error = ex.Message;

            }


            return resultado;

        }


        // Lista todos los productos existentes
        public async Task<Respuesta<IEnumerable<TProducto>>> ListarAsync()

        {

            var resultado =

                new Respuesta<IEnumerable<TProducto>>();


            try
            {

                // Obtiene todos los productos desde el repositorio
                var resp =

                    await _unidadDeTrabajo.TProducto.ListarAsync();


                // Convierte la lista de entidades de dominio a DTOs tipados
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


        // Modifica un producto existente, validando primero que exista
        public async Task<Respuesta<TProducto>> ModificarAsync(TProducto datos)

        {

            var resultado =

                new Respuesta<TProducto>();


            try
            {

                // Busca el producto actual en base de datos por su Id
                var productoActual =

                    await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(

                        x => x.ProductoId == datos.ProductoId);


                if (productoActual.Data == null)

                {

                    resultado.Error =

                        "No existe el producto a modificar.";


                    return resultado;

                }





                // Copia los valores del DTO recibido sobre la entidad existente rastreada por EF
                _mapper.Map(datos, productoActual.Data);


                // Guarda los cambios en el repositorio
                var respuestaRepositorio =

                    await _unidadDeTrabajo.TProducto.ModificarAsync(

                        productoActual.Data);


                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();


                // Convierte la entidad modificada de vuelta a DTO tipado para la respuesta
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


        // Elimina un producto existente, validando primero que exista
        public async Task<Respuesta<bool>> EliminarAsync(TProducto datos)

        {

            var resultado =

                new Respuesta<bool>();


            try
            {

                // Busca el producto a eliminar por su Id
                var producto =

                    await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(

                        x => x.ProductoId == datos.ProductoId);


                if (producto.Data == null)

                {

                    resultado.Error =

                        "No existe el producto a eliminar.";


                    return resultado;

                }


                // Elimina la entidad del repositorio
                var respuestaRepositorio =

                    await _unidadDeTrabajo.TProducto.EliminarAsync(

                        producto.Data);


                // Confirma (commit) los cambios en la unidad de trabajo
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


        // Busca productos cuyo nombre contenga el texto recibido
        public async Task<Respuesta<IEnumerable<TProducto>>> BuscarAsync(

            TProducto datos)

        {

            var resultado =

                new Respuesta<IEnumerable<TProducto>>();


            try
            {

                // Filtra los productos cuyo nombre contenga el texto de búsqueda
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


        // Obtiene un producto puntual según su Id
        public async Task<Respuesta<TProducto>> ObtenerAsync(

            TProducto datos)

        {

            var resultado =

                new Respuesta<TProducto>();


            try
            {

                // Busca el producto por su Id
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