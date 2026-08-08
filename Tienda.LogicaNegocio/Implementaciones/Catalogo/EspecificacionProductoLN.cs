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
    // Implementación de la lógica de negocio para "EspecificacionProducto"
    // (las especificaciones/características técnicas asociadas a un producto).
    // Nota: a diferencia de GarantiaLN y EtiquetaLN, esta clase NO implementa
    // ModificarAsync, ListarAsync ni BuscarAsync genéricos; en su lugar expone
    // un método específico ListarPorProductoAsync.
    public class EspecificacionProductoLN : IEspecificacionProductoLN
    {
        // Unidad de trabajo (patrón Unit of Work) que agrupa los repositorios de acceso a datos
        // y permite controlar transacciones (guardar varios cambios de forma atómica).
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }

        // Logger para registrar errores u otra información relevante durante la ejecución.
        private ILogger<EspecificacionProductoLN> _logger { get; }

        // AutoMapper: se usa para convertir entre la entidad de base de datos (EspecificacionProducto)
        // y el DTO/modelo tipado usado en la capa de negocio (TEspecificacionProducto).
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias por inyección de dependencias (DI).
        public EspecificacionProductoLN(IUnidadTrabajoEF unidadTrabajo, ILogger<EspecificacionProductoLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta una nueva especificación para un producto.
        public async Task<Respuesta<TEspecificacionProducto>> InsertarAsync(TEspecificacionProducto datos)
        {
            var resultado = new Respuesta<TEspecificacionProducto>();
            try
            {
                // Validación previa: se comprueba que el producto al que pertenece
                // la especificación exista realmente antes de insertarla (integridad referencial
                // a nivel de lógica de negocio).
                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                // Se mapea el DTO (TEspecificacionProducto) a la entidad de base de datos (EspecificacionProducto).
                // Nota: a diferencia de GarantiaLN/EtiquetaLN, aquí no se asigna un campo CreadoEn.
                var entidad = _mapper.Map<EspecificacionProducto>(datos);

                // Se inserta la entidad usando el repositorio correspondiente dentro de la unidad de trabajo.
                var respuesta = await _unidadDeTrabajo.TEspecificacionProducto.InsertarAsync(entidad);

                // Se confirma (commit) el cambio en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se mapea la entidad insertada de vuelta al DTO para devolverla al llamador.
                resultado.Data = _mapper.Map<TEspecificacionProducto>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se registra en el log junto con el ProductoId
                // y se guarda el mensaje de error en la respuesta.
                _logger.LogError(ex, "Error al insertar especificación del producto {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Elimina una especificación de producto existente.
        public async Task<Respuesta<bool>> EliminarAsync(TEspecificacionProducto datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Se busca el registro a eliminar por su EspecificacionId.
                var registro = await _unidadDeTrabajo.TEspecificacionProducto.ObtenerEntidadAsync(x => x.EspecificacionId == datos.EspecificacionId);

                // Si no existe, se informa el error y se detiene el proceso.
                if (registro.Data == null)
                {
                    resultado.Error = "No existe la especificación a eliminar.";
                    return resultado;
                }

                // Se elimina la entidad encontrada.
                var respuesta = await _unidadDeTrabajo.TEspecificacionProducto.EliminarAsync(registro.Data);

                // Se confirma (commit) la eliminación en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se guarda el resultado booleano indicando si la eliminación fue exitosa.
                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar EspecificacionId {Id}", datos.EspecificacionId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Lista todas las especificaciones asociadas a un producto específico,
        // ordenadas por su campo "Orden" (por ejemplo, para mostrarlas en un orden definido en la UI).
        public async Task<Respuesta<IEnumerable<TEspecificacionProducto>>> ListarPorProductoAsync(int productoId)
        {
            var resultado = new Respuesta<IEnumerable<TEspecificacionProducto>>();
            try
            {
                // Se buscan todas las especificaciones cuyo ProductoId coincida con el parámetro recibido.
                var respuesta = await _unidadDeTrabajo.TEspecificacionProducto.BuscarAsync(x => x.ProductoId == productoId);

                // Se ordenan los resultados por el campo "Orden". Si respuesta.Data viene null,
                // se usa una colección vacía como respaldo para evitar una excepción de referencia nula.
                var ordenado = (respuesta.Data ?? Enumerable.Empty<EspecificacionProducto>()).OrderBy(e => e.Orden);

                // Se mapea la colección ya ordenada de entidades a una colección de DTOs.
                resultado.Data = _mapper.Map<IEnumerable<TEspecificacionProducto>>(ordenado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar especificaciones del producto {ProductoId}", productoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}