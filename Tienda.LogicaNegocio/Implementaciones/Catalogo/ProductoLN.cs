using AutoMapper;
using Microsoft.Extensions.Logging;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.LogicaNegocio.Implementaciones
{
    public class ProductoLN : IProductoLN
    {
        private readonly IUnidadTrabajoEF _unidadDeTrabajo;
        private readonly ILogger<ProductoLN> _logger;
        private readonly IMapper _mapper;

        public ProductoLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ProductoLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TProducto>> InsertarAsync(TCrearProductoConInventario datos)
        {
            var resultado = new Respuesta<TProducto>();
            var transaccionActiva = false;

            try
            {
                var nombre = datos.Nombre?.Trim() ?? string.Empty;
                var descripcion = NormalizarDescripcion(datos.Descripcion);
                var errorValidacion = ValidarDatosComerciales(nombre, descripcion, datos.Precio, datos.CostoCompra);
                if (errorValidacion != null)
                {
                    resultado.Error = errorValidacion;
                    return resultado;
                }

                if (datos.CantidadInicial < 0)
                {
                    resultado.Error = "La cantidad inicial no puede ser negativa.";
                    return resultado;
                }

                var errorRelaciones = await ValidarRelacionesAsync(
                    datos.SubcategoriaId,
                    datos.MarcaId,
                    datos.ProveedorId,
                    datos.BodegaId);
                if (errorRelaciones != null)
                {
                    resultado.Error = errorRelaciones;
                    return resultado;
                }

                var productoExistente = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.Nombre == nombre);
                if (!string.IsNullOrEmpty(productoExistente.Error))
                {
                    resultado.Error = "No fue posible validar el nombre del producto.";
                    return resultado;
                }

                if (productoExistente.Data != null)
                {
                    resultado.Error = "Ya existe un producto registrado con ese nombre.";
                    return resultado;
                }

                _unidadDeTrabajo.EmpezarTransaccion();
                transaccionActiva = true;

                var producto = new Producto
                {
                    Nombre = nombre,
                    Descripcion = descripcion,
                    Precio = datos.Precio,
                    CostoCompra = datos.CostoCompra,
                    SubcategoriaId = datos.SubcategoriaId,
                    MarcaId = datos.MarcaId,
                    ProveedorId = datos.ProveedorId,
                    Activo = true
                };

                var respuestaProducto = await _unidadDeTrabajo.TProducto.InsertarAsync(producto);
                if (!string.IsNullOrEmpty(respuestaProducto.Error) || respuestaProducto.Data == null)
                {
                    RevertirTransaccion(ref transaccionActiva);
                    resultado.Error = "No fue posible crear el producto.";
                    return resultado;
                }

                var inventario = new Inventario
                {
                    ProductoId = respuestaProducto.Data.ProductoId,
                    BodegaId = datos.BodegaId,
                    Cantidad = datos.CantidadInicial
                };

                var respuestaInventario = await _unidadDeTrabajo.TInventario.InsertarAsync(inventario);
                if (!string.IsNullOrEmpty(respuestaInventario.Error) || respuestaInventario.Data == null)
                {
                    RevertirTransaccion(ref transaccionActiva);
                    resultado.Error = "No fue posible crear el inventario inicial del producto.";
                    return resultado;
                }

                // CompletarTran confirma ambos registros como una sola operación.
                transaccionActiva = false;
                _unidadDeTrabajo.CompletarTran();

                resultado.Data = _mapper.Map<TProducto>(respuestaProducto.Data);
            }
            catch (Exception ex)
            {
                RevertirTransaccion(ref transaccionActiva);
                _logger.LogError(ex, "Error al crear producto e inventario inicial.");
                resultado.Error = "No fue posible registrar el producto.";
            }

            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TProducto>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TProducto>>();

            try
            {
                var respuesta = await _unidadDeTrabajo.TProducto.BuscarAsync(x => x.Activo);
                if (!string.IsNullOrEmpty(respuesta.Error))
                {
                    resultado.Error = "No fue posible listar los productos.";
                    return resultado;
                }

                resultado.Data = _mapper.Map<IEnumerable<TProducto>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar productos activos.");
                resultado.Error = "No fue posible listar los productos.";
            }

            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TProducto>>> ListarAdministracionAsync()
        {
            var resultado = new Respuesta<IEnumerable<TProducto>>();

            try
            {
                var respuesta = await _unidadDeTrabajo.TProducto.ListarAsync();
                if (!string.IsNullOrEmpty(respuesta.Error))
                {
                    resultado.Error = "No fue posible listar los productos.";
                    return resultado;
                }

                resultado.Data = _mapper.Map<IEnumerable<TProducto>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar productos para administración.");
                resultado.Error = "No fue posible listar los productos.";
            }

            return resultado;
        }

        public async Task<Respuesta<TProducto>> ModificarAsync(TActualizarProducto datos)
        {
            var resultado = new Respuesta<TProducto>();

            try
            {
                if (datos.ProductoId <= 0)
                {
                    resultado.Error = "El producto indicado no es válido.";
                    return resultado;
                }

                var nombre = datos.Nombre?.Trim() ?? string.Empty;
                var descripcion = NormalizarDescripcion(datos.Descripcion);
                var errorValidacion = ValidarDatosComerciales(nombre, descripcion, datos.Precio, datos.CostoCompra);
                if (errorValidacion != null)
                {
                    resultado.Error = errorValidacion;
                    return resultado;
                }

                var productoActual = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (productoActual.Data == null)
                {
                    resultado.Error = "No existe el producto a modificar.";
                    return resultado;
                }

                var errorRelaciones = await ValidarRelacionesAsync(
                    datos.SubcategoriaId,
                    datos.MarcaId,
                    datos.ProveedorId);
                if (errorRelaciones != null)
                {
                    resultado.Error = errorRelaciones;
                    return resultado;
                }

                var productoConMismoNombre = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(
                    x => x.Nombre == nombre && x.ProductoId != datos.ProductoId);
                if (!string.IsNullOrEmpty(productoConMismoNombre.Error))
                {
                    resultado.Error = "No fue posible validar el nombre del producto.";
                    return resultado;
                }

                if (productoConMismoNombre.Data != null)
                {
                    resultado.Error = "Ya existe un producto registrado con ese nombre.";
                    return resultado;
                }

                productoActual.Data.Nombre = nombre;
                productoActual.Data.Descripcion = descripcion;
                productoActual.Data.Precio = datos.Precio;
                productoActual.Data.CostoCompra = datos.CostoCompra;
                productoActual.Data.SubcategoriaId = datos.SubcategoriaId;
                productoActual.Data.MarcaId = datos.MarcaId;
                productoActual.Data.ProveedorId = datos.ProveedorId;

                var respuesta = await _unidadDeTrabajo.TProducto.ModificarAsync(productoActual.Data);
                if (!string.IsNullOrEmpty(respuesta.Error) || respuesta.Data == null)
                {
                    resultado.Error = "No fue posible actualizar el producto.";
                    return resultado;
                }

                _unidadDeTrabajo.Completar();
                resultado.Data = _mapper.Map<TProducto>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar ProductoId {ProductoId}", datos.ProductoId);
                resultado.Error = "No fue posible actualizar el producto.";
            }

            return resultado;
        }

        public async Task<Respuesta<TProducto>> CambiarEstadoAsync(TCambiarEstadoProducto datos)
        {
            var resultado = new Respuesta<TProducto>();

            try
            {
                if (datos.ProductoId <= 0)
                {
                    resultado.Error = "El producto indicado no es válido.";
                    return resultado;
                }

                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "No existe el producto a actualizar.";
                    return resultado;
                }

                producto.Data.Activo = datos.Activo;
                var respuesta = await _unidadDeTrabajo.TProducto.ModificarAsync(producto.Data);
                if (!string.IsNullOrEmpty(respuesta.Error) || respuesta.Data == null)
                {
                    resultado.Error = "No fue posible actualizar el estado del producto.";
                    return resultado;
                }

                _unidadDeTrabajo.Completar();
                resultado.Data = _mapper.Map<TProducto>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cambiar estado de ProductoId {ProductoId}", datos.ProductoId);
                resultado.Error = "No fue posible actualizar el estado del producto.";
            }

            return resultado;
        }

        public Task<Respuesta<bool>> EliminarAsync(TProducto datos)
        {
            return Task.FromResult(new Respuesta<bool>
            {
                Error = "No se permite eliminar productos físicamente. Utilice la desactivación."
            });
        }

        public async Task<Respuesta<IEnumerable<TProducto>>> BuscarAsync(TProducto datos)
        {
            var resultado = new Respuesta<IEnumerable<TProducto>>();

            try
            {
                var nombre = datos.Nombre?.Trim() ?? string.Empty;
                var respuesta = await _unidadDeTrabajo.TProducto.BuscarAsync(
                    x => x.Activo && x.Nombre.Contains(nombre));
                if (!string.IsNullOrEmpty(respuesta.Error))
                {
                    resultado.Error = "No fue posible buscar los productos.";
                    return resultado;
                }

                resultado.Data = _mapper.Map<IEnumerable<TProducto>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar productos activos.");
                resultado.Error = "No fue posible buscar los productos.";
            }

            return resultado;
        }

        public async Task<Respuesta<TProducto>> ObtenerAsync(TProducto datos)
        {
            var resultado = new Respuesta<TProducto>();

            try
            {
                var respuesta = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(
                    x => x.ProductoId == datos.ProductoId && x.Activo);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Producto no encontrado.";
                    return resultado;
                }

                resultado.Data = _mapper.Map<TProducto>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ProductoId {ProductoId}", datos.ProductoId);
                resultado.Error = "Producto no encontrado.";
            }

            return resultado;
        }

        private async Task<string?> ValidarRelacionesAsync(
            int subcategoriaId,
            int marcaId,
            int proveedorId,
            int? bodegaId = null)
        {
            if (subcategoriaId <= 0 || marcaId <= 0 || proveedorId <= 0 || (bodegaId.HasValue && bodegaId <= 0))
            {
                return "Las relaciones del producto no son válidas.";
            }

            var subcategoria = await _unidadDeTrabajo.TSubcategoria.ObtenerEntidadAsync(x => x.SubcategoriaId == subcategoriaId);
            if (!string.IsNullOrEmpty(subcategoria.Error) || subcategoria.Data == null)
            {
                return "La subcategoría indicada no existe.";
            }

            var marca = await _unidadDeTrabajo.TMarca.ObtenerEntidadAsync(x => x.MarcaId == marcaId);
            if (!string.IsNullOrEmpty(marca.Error) || marca.Data == null)
            {
                return "La marca indicada no existe.";
            }

            var proveedor = await _unidadDeTrabajo.TProveedor.ObtenerEntidadAsync(x => x.ProveedorId == proveedorId);
            if (!string.IsNullOrEmpty(proveedor.Error) || proveedor.Data == null)
            {
                return "El proveedor indicado no existe.";
            }

            if (bodegaId.HasValue)
            {
                var bodega = await _unidadDeTrabajo.TBodega.ObtenerEntidadAsync(x => x.BodegaId == bodegaId.Value);
                if (!string.IsNullOrEmpty(bodega.Error) || bodega.Data == null)
                {
                    return "La bodega indicada no existe.";
                }
            }

            return null;
        }

        private static string? ValidarDatosComerciales(string nombre, string? descripcion, decimal precio, decimal? costoCompra)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                return "El nombre del producto es obligatorio.";
            }

            if (nombre.Length > 150)
            {
                return "El nombre del producto no puede superar los 150 caracteres.";
            }

            if (descripcion != null && descripcion.Length > 500)
            {
                return "La descripción no puede superar los 500 caracteres.";
            }

            if (precio <= 0)
            {
                return "El precio debe ser mayor que cero.";
            }

            if (costoCompra.HasValue && costoCompra.Value < 0)
            {
                return "El costo de compra no puede ser negativo.";
            }

            return null;
        }

        private static string? NormalizarDescripcion(string? descripcion)
        {
            var descripcionNormalizada = descripcion?.Trim();
            return string.IsNullOrWhiteSpace(descripcionNormalizada) ? null : descripcionNormalizada;
        }

        private void RevertirTransaccion(ref bool transaccionActiva)
        {
            if (!transaccionActiva)
            {
                return;
            }

            try
            {
                _unidadDeTrabajo.Rollback();
            }
            finally
            {
                transaccionActiva = false;
            }
        }
    }
}
