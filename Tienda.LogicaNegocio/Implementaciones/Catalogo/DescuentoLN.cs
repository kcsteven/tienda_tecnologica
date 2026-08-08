using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.LogicaNegocio.Implementaciones
{
    // Implementación de la lógica de negocio para la entidad "Descuento".
    // Implementa la interfaz IDescuentoLN, que define el contrato de operaciones disponibles.
    // Sigue el mismo patrón CRUD que EtiquetaLN.
    public class DescuentoLN : IDescuentoLN
    {
        // Unidad de trabajo (patrón Unit of Work) que agrupa los repositorios de acceso a datos
        // y permite controlar transacciones (guardar varios cambios de forma atómica).
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }

        // Logger para registrar errores u otra información relevante durante la ejecución.
        private ILogger<DescuentoLN> _logger { get; }

        // AutoMapper: se usa para convertir entre la entidad de base de datos (Descuento)
        // y el DTO/modelo tipado usado en la capa de negocio (TDescuento).
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias por inyección de dependencias (DI).
        public DescuentoLN(IUnidadTrabajoEF unidadTrabajo, ILogger<DescuentoLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta un nuevo descuento en la base de datos.
        public async Task<Respuesta<TDescuento>> InsertarAsync(TDescuento datos)
        {
            var resultado = new Respuesta<TDescuento>();
            try
            {
                // Validación previa: se comprueba si ya existe un descuento con el mismo nombre,
                // para evitar duplicados.
                var existente = await _unidadDeTrabajo.TDescuento.ObtenerEntidadAsync(x => x.Nombre == datos.Nombre);
                if (existente.Data != null)
                {
                    resultado.Error = "Ya existe un descuento registrado con ese nombre.";
                    return resultado;
                }

                // Se asigna la fecha de creación en UTC antes de guardar.
                datos.CreadoEn = DateTime.UtcNow;

                // Se mapea el DTO (TDescuento) a la entidad de base de datos (Descuento).
                var entidad = _mapper.Map<Descuento>(datos);

                // Se inserta la entidad usando el repositorio correspondiente dentro de la unidad de trabajo.
                var respuesta = await _unidadDeTrabajo.TDescuento.InsertarAsync(entidad);

                // Se confirma (commit) el cambio en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se mapea la entidad insertada de vuelta al DTO para devolverla al llamador.
                resultado.Data = _mapper.Map<TDescuento>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se registra en el log junto con el nombre del descuento
                // y se guarda el mensaje de error en la respuesta.
                _logger.LogError(ex, "Error al insertar descuento {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Modifica (actualiza) un descuento existente.
        public async Task<Respuesta<TDescuento>> ModificarAsync(TDescuento datos)
        {
            var resultado = new Respuesta<TDescuento>();
            try
            {
                // Se busca el descuento actual en la base de datos por su Id.
                var actual = await _unidadDeTrabajo.TDescuento.ObtenerEntidadAsync(x => x.DescuentoId == datos.DescuentoId);

                // Si no existe, se devuelve un error y se corta la ejecución.
                if (actual.Data == null)
                {
                    resultado.Error = "No existe el descuento a modificar.";
                    return resultado;
                }

                // Se actualiza la fecha de modificación en UTC.
                datos.ActualizadoEn = DateTime.UtcNow;

                // Se copian (mapean) los valores nuevos de "datos" sobre la entidad "actual"
                // que ya está siendo rastreada por el contexto de base de datos.
                _mapper.Map(datos, actual.Data);

                // Se guarda la entidad ya modificada.
                var respuesta = await _unidadDeTrabajo.TDescuento.ModificarAsync(actual.Data);

                // Se confirman los cambios en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se mapea la entidad actualizada de vuelta al DTO de salida.
                resultado.Data = _mapper.Map<TDescuento>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar DescuentoId {DescuentoId}", datos.DescuentoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Elimina un descuento existente.
        public async Task<Respuesta<bool>> EliminarAsync(TDescuento datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Se busca el descuento a eliminar por su Id.
                var descuento = await _unidadDeTrabajo.TDescuento.ObtenerEntidadAsync(x => x.DescuentoId == datos.DescuentoId);

                // Si no existe, se informa el error y se detiene el proceso.
                if (descuento.Data == null)
                {
                    resultado.Error = "No existe el descuento a eliminar.";
                    return resultado;
                }

                // Se elimina la entidad encontrada.
                var respuesta = await _unidadDeTrabajo.TDescuento.EliminarAsync(descuento.Data);

                // Se confirma (commit) la eliminación en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se guarda el resultado booleano indicando si la eliminación fue exitosa.
                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar DescuentoId {DescuentoId}", datos.DescuentoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Lista todos los descuentos registrados.
        public async Task<Respuesta<IEnumerable<TDescuento>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TDescuento>>();
            try
            {
                // Se obtienen todos los descuentos desde el repositorio.
                var resp = await _unidadDeTrabajo.TDescuento.ListarAsync();

                // Se mapea la colección de entidades a una colección de DTOs.
                resultado.Data = _mapper.Map<IEnumerable<TDescuento>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar descuentos.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Busca descuentos cuyo nombre contenga el texto indicado en "datos.Nombre".
        // Nota: no hay protección contra datos.Nombre nulo (igual que en EtiquetaLN);
        // si llega null, Contains lanzaría una excepción que quedaría capturada por el catch.
        public async Task<Respuesta<IEnumerable<TDescuento>>> BuscarAsync(TDescuento datos)
        {
            var resultado = new Respuesta<IEnumerable<TDescuento>>();
            try
            {
                // Filtro: nombre que contenga el texto buscado.
                var respuesta = await _unidadDeTrabajo.TDescuento.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));

                // Se mapea el resultado a una colección de DTOs.
                resultado.Data = _mapper.Map<IEnumerable<TDescuento>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar descuentos.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Obtiene un único descuento por su Id.
        public async Task<Respuesta<TDescuento>> ObtenerAsync(TDescuento datos)
        {
            var resultado = new Respuesta<TDescuento>();
            try
            {
                // Se busca la entidad por DescuentoId.
                var respuesta = await _unidadDeTrabajo.TDescuento.ObtenerEntidadAsync(x => x.DescuentoId == datos.DescuentoId);

                // Si no se encuentra, se retorna un mensaje de error.
                if (respuesta.Data == null)
                {
                    resultado.Error = "Descuento no encontrado.";
                    return resultado;
                }

                // Se mapea la entidad encontrada al DTO de salida.
                resultado.Data = _mapper.Map<TDescuento>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener DescuentoId {DescuentoId}", datos.DescuentoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}