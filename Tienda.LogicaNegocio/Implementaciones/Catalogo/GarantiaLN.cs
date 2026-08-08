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
    // Implementación de la lógica de negocio para la entidad "Garantía".
    // Implementa la interfaz IGarantiaLN, que define el contrato de operaciones disponibles.
    public class GarantiaLN : IGarantiaLN
    {
        // Unidad de trabajo (patrón Unit of Work) que agrupa los repositorios de acceso a datos
        // y permite controlar transacciones (guardar varios cambios de forma atómica).
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }

        // Logger para registrar errores u otra información relevante durante la ejecución.
        private ILogger<GarantiaLN> _logger { get; }

        // AutoMapper: se usa para convertir entre la entidad de base de datos (Garantia)
        // y el DTO/modelo tipado usado en la capa de negocio (TGarantia).
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias por inyección de dependencias (DI).
        public GarantiaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<GarantiaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta una nueva garantía en la base de datos.
        public async Task<Respuesta<TGarantia>> InsertarAsync(TGarantia datos)
        {
            // Objeto de respuesta genérico que contendrá el resultado o el error.
            var resultado = new Respuesta<TGarantia>();
            try
            {
                // Se asigna la fecha de creación en UTC antes de guardar.
                datos.CreadoEn = DateTime.UtcNow;

                // Se mapea el DTO (TGarantia) a la entidad de base de datos (Garantia).
                var entidad = _mapper.Map<Garantia>(datos);

                // Se inserta la entidad usando el repositorio correspondiente dentro de la unidad de trabajo.
                var respuesta = await _unidadDeTrabajo.TGarantia.InsertarAsync(entidad);

                // Se confirma (commit) el cambio en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se mapea la entidad insertada de vuelta al DTO para devolverla al llamador.
                resultado.Data = _mapper.Map<TGarantia>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se registra en el log junto con el nombre de la garantía
                // y se guarda el mensaje de error en la respuesta.
                _logger.LogError(ex, "Error al insertar garantía {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Modifica (actualiza) una garantía existente.
        public async Task<Respuesta<TGarantia>> ModificarAsync(TGarantia datos)
        {
            var resultado = new Respuesta<TGarantia>();
            try
            {
                // Primero se busca la garantía actual en la base de datos por su Id.
                var actual = await _unidadDeTrabajo.TGarantia.ObtenerEntidadAsync(x => x.GarantiaId == datos.GarantiaId);

                // Si no existe, se devuelve un error y se corta la ejecución.
                if (actual.Data == null)
                {
                    resultado.Error = "No existe la garantía a modificar.";
                    return resultado;
                }

                // Se actualiza la fecha de modificación en UTC.
                datos.ActualizadoEn = DateTime.UtcNow;

                // Se copian (mapean) los valores nuevos de "datos" sobre la entidad "actual"
                // que ya está siendo rastreada por el contexto de base de datos.
                _mapper.Map(datos, actual.Data);

                // Se guarda la entidad ya modificada.
                var respuesta = await _unidadDeTrabajo.TGarantia.ModificarAsync(actual.Data);

                // Se confirman los cambios en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se mapea la entidad actualizada de vuelta al DTO de salida.
                resultado.Data = _mapper.Map<TGarantia>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar GarantiaId {GarantiaId}", datos.GarantiaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Elimina una garantía existente.
        public async Task<Respuesta<bool>> EliminarAsync(TGarantia datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Se busca la garantía a eliminar por su Id.
                var garantia = await _unidadDeTrabajo.TGarantia.ObtenerEntidadAsync(x => x.GarantiaId == datos.GarantiaId);

                // Si no existe, se informa el error y se detiene el proceso.
                if (garantia.Data == null)
                {
                    resultado.Error = "No existe la garantía a eliminar.";
                    return resultado;
                }

                // Se elimina la entidad encontrada.
                var respuesta = await _unidadDeTrabajo.TGarantia.EliminarAsync(garantia.Data);

                // Se confirma (commit) la eliminación en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se guarda el resultado booleano indicando si la eliminación fue exitosa.
                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar GarantiaId {GarantiaId}", datos.GarantiaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Lista todas las garantías registradas.
        public async Task<Respuesta<IEnumerable<TGarantia>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TGarantia>>();
            try
            {
                // Se obtienen todas las garantías desde el repositorio.
                var resp = await _unidadDeTrabajo.TGarantia.ListarAsync();

                // Se mapea la colección de entidades a una colección de DTOs.
                resultado.Data = _mapper.Map<IEnumerable<TGarantia>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar garantías.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Busca garantías cuyo nombre contenga el texto indicado en "datos.Nombre".
        public async Task<Respuesta<IEnumerable<TGarantia>>> BuscarAsync(TGarantia datos)
        {
            var resultado = new Respuesta<IEnumerable<TGarantia>>();
            try
            {
                // Filtro: nombre no nulo y que contenga el texto buscado (o vacío si no se envía nada).
                var respuesta = await _unidadDeTrabajo.TGarantia.BuscarAsync(x => x.Nombre != null && x.Nombre.Contains(datos.Nombre ?? ""));

                // Se mapea el resultado a una colección de DTOs.
                resultado.Data = _mapper.Map<IEnumerable<TGarantia>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar garantías.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Obtiene una única garantía por su Id.
        public async Task<Respuesta<TGarantia>> ObtenerAsync(TGarantia datos)
        {
            var resultado = new Respuesta<TGarantia>();
            try
            {
                // Se busca la entidad por GarantiaId.
                var respuesta = await _unidadDeTrabajo.TGarantia.ObtenerEntidadAsync(x => x.GarantiaId == datos.GarantiaId);

                // Si no se encuentra, se retorna un mensaje de error.
                if (respuesta.Data == null)
                {
                    resultado.Error = "Garantía no encontrada.";
                    return resultado;
                }

                // Se mapea la entidad encontrada al DTO de salida.
                resultado.Data = _mapper.Map<TGarantia>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener GarantiaId {GarantiaId}", datos.GarantiaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}