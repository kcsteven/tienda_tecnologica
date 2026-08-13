using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.LogicaNegocio.Implementaciones
{
    // Implementación de la lógica de negocio (LN) para la entidad Inventario
    public class InventarioLN : IInventarioLN
    {
        // Unidad de trabajo (Entity Framework) para acceder a los repositorios de datos
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        // Logger para registrar errores y eventos de esta clase
        private ILogger<InventarioLN> _logger { get; }
        // AutoMapper para convertir entre entidades de dominio (Inventario) y entidades tipadas (TInventario)
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias mediante inyección de dependencias
        public InventarioLN(IUnidadTrabajoEF unidadTrabajo, ILogger<InventarioLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta un nuevo registro de inventario, validando que el producto y la bodega indicados existan
        public async Task<Respuesta<TInventario>> InsertarAsync(TInventario datos)
        {
            var resultado = new Respuesta<TInventario>();
            try
            {
                if (datos.Cantidad < 0)
                {
                    resultado.Error = "La cantidad de inventario no puede ser negativa.";
                    return resultado;
                }

                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                // Verifica que la bodega indicada exista
                var bodega = await _unidadDeTrabajo.TBodega.ObtenerEntidadAsync(x => x.BodegaId == datos.BodegaId);
                if (bodega.Data == null)
                {
                    resultado.Error = "La bodega indicada no existe.";
                    return resultado;
                }

                // Convierte el DTO tipado a la entidad de dominio
                var entidad = _mapper.Map<Inventario>(datos);
                // Inserta la entidad en el repositorio
                var respuesta = await _unidadDeTrabajo.TInventario.InsertarAsync(entidad);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                // Convierte la entidad insertada de vuelta a DTO tipado para la respuesta
                resultado.Data = _mapper.Map<TInventario>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Registra el error y lo devuelve en la respuesta
                _logger.LogError(ex, "Error al insertar inventario del producto {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TInventario>> ModificarAsync(TAjustarInventario datos)
        {
            var resultado = new Respuesta<TInventario>();
            try
            {
                if (datos.InventarioId <= 0 || datos.Cantidad < 0)
                {
                    resultado.Error = "El ajuste de inventario no es válido.";
                    return resultado;
                }

                var actual = await _unidadDeTrabajo.TInventario.ObtenerEntidadAsync(x => x.InventarioId == datos.InventarioId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe el registro de inventario a modificar.";
                    return resultado;
                }

                // El ajuste administrativo modifica solamente la cantidad existente.
                actual.Data.Cantidad = datos.Cantidad;

                // Guarda los cambios en el repositorio
                var respuesta = await _unidadDeTrabajo.TInventario.ModificarAsync(actual.Data);
                if (!string.IsNullOrEmpty(respuesta.Error) || respuesta.Data == null)
                {
                    resultado.Error = "No fue posible actualizar el inventario.";
                    return resultado;
                }

                _unidadDeTrabajo.Completar();

                // Convierte la entidad modificada de vuelta a DTO tipado para la respuesta
                resultado.Data = _mapper.Map<TInventario>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar InventarioId {InventarioId}", datos.InventarioId);
                resultado.Error = "No fue posible actualizar el inventario.";
            }
            return resultado;
        }

        // Elimina un registro de inventario existente, validando primero que exista
        public async Task<Respuesta<bool>> EliminarAsync(TInventario datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Busca el registro de inventario a eliminar por su Id
                var inventario = await _unidadDeTrabajo.TInventario.ObtenerEntidadAsync(x => x.InventarioId == datos.InventarioId);
                if (inventario.Data == null)
                {
                    resultado.Error = "No existe el registro de inventario a eliminar.";
                    return resultado;
                }

                // Elimina la entidad del repositorio
                var respuesta = await _unidadDeTrabajo.TInventario.EliminarAsync(inventario.Data);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar InventarioId {InventarioId}", datos.InventarioId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Lista todos los registros de inventario existentes
        public async Task<Respuesta<IEnumerable<TInventario>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TInventario>>();
            try
            {
                // Obtiene todos los registros de inventario desde el repositorio
                var resp = await _unidadDeTrabajo.TInventario.ListarAsync();
                // Convierte la lista de entidades de dominio a DTOs tipados
                resultado.Data = _mapper.Map<IEnumerable<TInventario>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar inventario.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Lista los registros de inventario asociados a un producto específico
        public async Task<Respuesta<IEnumerable<TInventario>>> ListarPorProductoAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TInventario>>();
            try
            {
                // Filtra el inventario por el Id de producto recibido
                var respuesta = await _unidadDeTrabajo.TInventario.BuscarAsync(x => x.ProductoId == productoId);
                resultado.Data = _mapper.Map<IEnumerable<TInventario>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar inventario del producto {ProductoId}", productoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TDisponibilidadBodega>>> ListarDisponibilidadPublicaAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TDisponibilidadBodega>>();
            try
            {
                if (productoId <= 0)
                {
                    resultado.Data = Array.Empty<TDisponibilidadBodega>();
                    return resultado;
                }

                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(
                    x => x.ProductoId == productoId && x.Activo);
                if (!string.IsNullOrEmpty(producto.Error))
                {
                    resultado.Error = "No fue posible consultar la disponibilidad.";
                    return resultado;
                }

                if (producto.Data == null)
                {
                    resultado.Data = Array.Empty<TDisponibilidadBodega>();
                    return resultado;
                }

                // La inclusión evita consultar una bodega adicional por cada inventario.
                var inventarios = await _unidadDeTrabajo.TInventario.BuscarAsync(
                    x => x.ProductoId == productoId && x.Cantidad > 0,
                    new List<string> { nameof(Inventario.Bodega) });
                if (!string.IsNullOrEmpty(inventarios.Error))
                {
                    resultado.Error = "No fue posible consultar la disponibilidad.";
                    return resultado;
                }

                resultado.Data = (inventarios.Data ?? Enumerable.Empty<Inventario>())
                    .Where(inventario => inventario.Bodega != null && inventario.Bodega.Activo)
                    .Select(inventario => new TDisponibilidadBodega
                    {
                        NombreBodega = inventario.Bodega.Nombre,
                        Ubicacion = inventario.Bodega.Ubicacion,
                        Cantidad = inventario.Cantidad
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar disponibilidad pública para ProductoId {ProductoId}", productoId);
                resultado.Error = "No fue posible consultar la disponibilidad.";
            }

            return resultado;
        }

        public async Task<Respuesta<TInventario>> ObtenerAsync(TInventario datos)
        {
            var resultado = new Respuesta<TInventario>();
            try
            {
                // Busca el registro de inventario por su Id
                var respuesta = await _unidadDeTrabajo.TInventario.ObtenerEntidadAsync(x => x.InventarioId == datos.InventarioId);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Registro de inventario no encontrado.";
                    return resultado;
                }
                resultado.Data = _mapper.Map<TInventario>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener InventarioId {InventarioId}", datos.InventarioId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}
