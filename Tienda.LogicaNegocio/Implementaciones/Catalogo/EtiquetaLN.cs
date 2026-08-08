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
    // Implementación de la lógica de negocio para la entidad "Etiqueta".
    // Implementa la interfaz IEtiquetaLN, que define el contrato de operaciones disponibles.
    public class EtiquetaLN : IEtiquetaLN
    {
        // Unidad de trabajo (patrón Unit of Work) que agrupa los repositorios de acceso a datos
        // y permite controlar transacciones (guardar varios cambios de forma atómica).
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }

        // Logger para registrar errores u otra información relevante durante la ejecución.
        private ILogger<EtiquetaLN> _logger { get; }

        // AutoMapper: se usa para convertir entre la entidad de base de datos (Etiqueta)
        // y el DTO/modelo tipado usado en la capa de negocio (TEtiqueta).
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias por inyección de dependencias (DI).
        public EtiquetaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<EtiquetaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta una nueva etiqueta en la base de datos.
        public async Task<Respuesta<TEtiqueta>> InsertarAsync(TEtiqueta datos)
        {
            var resultado = new Respuesta<TEtiqueta>();
            try
            {
                // Validación previa: se comprueba si ya existe una etiqueta con el mismo nombre,
                // para evitar duplicados (a diferencia de GarantiaLN, aquí sí hay esta verificación).
                var existente = await _unidadDeTrabajo.TEtiqueta.ObtenerEntidadAsync(x => x.Nombre == datos.Nombre);
                if (existente.Data != null)
                {
                    resultado.Error = "Ya existe una etiqueta registrada con ese nombre.";
                    return resultado;
                }

                // Se asigna la fecha de creación en UTC antes de guardar.
                datos.CreadoEn = DateTime.UtcNow;

                // Se mapea el DTO (TEtiqueta) a la entidad de base de datos (Etiqueta).
                var entidad = _mapper.Map<Etiqueta>(datos);

                // Se inserta la entidad usando el repositorio correspondiente dentro de la unidad de trabajo.
                var respuesta = await _unidadDeTrabajo.TEtiqueta.InsertarAsync(entidad);

                // Se confirma (commit) el cambio en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se mapea la entidad insertada de vuelta al DTO para devolverla al llamador.
                resultado.Data = _mapper.Map<TEtiqueta>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se registra en el log junto con el nombre de la etiqueta
                // y se guarda el mensaje de error en la respuesta.
                _logger.LogError(ex, "Error al insertar etiqueta {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Modifica (actualiza) una etiqueta existente.
        public async Task<Respuesta<TEtiqueta>> ModificarAsync(TEtiqueta datos)
        {
            var resultado = new Respuesta<TEtiqueta>();
            try
            {
                // Se busca la etiqueta actual en la base de datos por su Id.
                var actual = await _unidadDeTrabajo.TEtiqueta.ObtenerEntidadAsync(x => x.EtiquetaId == datos.EtiquetaId);

                // Si no existe, se devuelve un error y se corta la ejecución.
                if (actual.Data == null)
                {
                    resultado.Error = "No existe la etiqueta a modificar.";
                    return resultado;
                }

                // Se actualiza la fecha de modificación en UTC.
                datos.ActualizadoEn = DateTime.UtcNow;

                // Se copian (mapean) los valores nuevos de "datos" sobre la entidad "actual"
                // que ya está siendo rastreada por el contexto de base de datos.
                _mapper.Map(datos, actual.Data);

                // Se guarda la entidad ya modificada.
                var respuesta = await _unidadDeTrabajo.TEtiqueta.ModificarAsync(actual.Data);

                // Se confirman los cambios en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se mapea la entidad actualizada de vuelta al DTO de salida.
                resultado.Data = _mapper.Map<TEtiqueta>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar EtiquetaId {EtiquetaId}", datos.EtiquetaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Elimina una etiqueta existente.
        public async Task<Respuesta<bool>> EliminarAsync(TEtiqueta datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Se busca la etiqueta a eliminar por su Id.
                var etiqueta = await _unidadDeTrabajo.TEtiqueta.ObtenerEntidadAsync(x => x.EtiquetaId == datos.EtiquetaId);

                // Si no existe, se informa el error y se detiene el proceso.
                if (etiqueta.Data == null)
                {
                    resultado.Error = "No existe la etiqueta a eliminar.";
                    return resultado;
                }

                // Se elimina la entidad encontrada.
                var respuesta = await _unidadDeTrabajo.TEtiqueta.EliminarAsync(etiqueta.Data);

                // Se confirma (commit) la eliminación en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se guarda el resultado booleano indicando si la eliminación fue exitosa.
                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar EtiquetaId {EtiquetaId}", datos.EtiquetaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Lista todas las etiquetas registradas.
        public async Task<Respuesta<IEnumerable<TEtiqueta>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TEtiqueta>>();
            try
            {
                // Se obtienen todas las etiquetas desde el repositorio.
                var resp = await _unidadDeTrabajo.TEtiqueta.ListarAsync();

                // Se mapea la colección de entidades a una colección de DTOs.
                resultado.Data = _mapper.Map<IEnumerable<TEtiqueta>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar etiquetas.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Busca etiquetas cuyo nombre contenga el texto indicado en "datos.Nombre".
        // Nota: a diferencia de GarantiaLN, aquí no se valida si datos.Nombre es null
        // antes de usar Contains, por lo que podría lanzar una excepción si es null
        // (aunque quedaría capturada por el catch).
        public async Task<Respuesta<IEnumerable<TEtiqueta>>> BuscarAsync(TEtiqueta datos)
        {
            var resultado = new Respuesta<IEnumerable<TEtiqueta>>();
            try
            {
                // Filtro: nombre que contenga el texto buscado.
                var respuesta = await _unidadDeTrabajo.TEtiqueta.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));

                // Se mapea el resultado a una colección de DTOs.
                resultado.Data = _mapper.Map<IEnumerable<TEtiqueta>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar etiquetas.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Obtiene una única etiqueta por su Id.
        public async Task<Respuesta<TEtiqueta>> ObtenerAsync(TEtiqueta datos)
        {
            var resultado = new Respuesta<TEtiqueta>();
            try
            {
                // Se busca la entidad por EtiquetaId.
                var respuesta = await _unidadDeTrabajo.TEtiqueta.ObtenerEntidadAsync(x => x.EtiquetaId == datos.EtiquetaId);

                // Si no se encuentra, se retorna un mensaje de error.
                if (respuesta.Data == null)
                {
                    resultado.Error = "Etiqueta no encontrada.";
                    return resultado;
                }

                // Se mapea la entidad encontrada al DTO de salida.
                resultado.Data = _mapper.Map<TEtiqueta>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener EtiquetaId {EtiquetaId}", datos.EtiquetaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}