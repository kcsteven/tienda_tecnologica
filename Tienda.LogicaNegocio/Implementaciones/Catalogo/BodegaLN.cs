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
    // Implementación de la lógica de negocio para la entidad "Bodega" (almacén/depósito).
    // Implementa la interfaz IBodegaLN, que define el contrato de operaciones disponibles.
    // Sigue el mismo patrón CRUD que EtiquetaLN y DescuentoLN.
    public class BodegaLN : IBodegaLN
    {
        // Unidad de trabajo (patrón Unit of Work) que agrupa los repositorios de acceso a datos
        // y permite controlar transacciones (guardar varios cambios de forma atómica).
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }

        // Logger para registrar errores u otra información relevante durante la ejecución.
        private ILogger<BodegaLN> _logger { get; }

        // AutoMapper: se usa para convertir entre la entidad de base de datos (Bodega)
        // y el DTO/modelo tipado usado en la capa de negocio (TBodega).
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias por inyección de dependencias (DI).
        public BodegaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<BodegaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta una nueva bodega en la base de datos.
        public async Task<Respuesta<TBodega>> InsertarAsync(TBodega datos)
        {
            var resultado = new Respuesta<TBodega>();
            try
            {
                // Validación previa: se comprueba si ya existe una bodega con el mismo nombre,
                // para evitar duplicados.
                var existente = await _unidadDeTrabajo.TBodega.ObtenerEntidadAsync(x => x.Nombre == datos.Nombre);
                if (existente.Data != null)
                {
                    resultado.Error = "Ya existe una bodega registrada con ese nombre.";
                    return resultado;
                }

                // Se asigna la fecha de creación en UTC antes de guardar.
                datos.CreadoEn = DateTime.UtcNow;

                // Se mapea el DTO (TBodega) a la entidad de base de datos (Bodega).
                var entidad = _mapper.Map<Bodega>(datos);

                // Se inserta la entidad usando el repositorio correspondiente dentro de la unidad de trabajo.
                var respuesta = await _unidadDeTrabajo.TBodega.InsertarAsync(entidad);

                // Se confirma (commit) el cambio en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se mapea la entidad insertada de vuelta al DTO para devolverla al llamador.
                resultado.Data = _mapper.Map<TBodega>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Si ocurre un error, se registra en el log junto con el nombre de la bodega
                // y se guarda el mensaje de error en la respuesta.
                _logger.LogError(ex, "Error al insertar bodega {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Modifica (actualiza) una bodega existente.
        public async Task<Respuesta<TBodega>> ModificarAsync(TBodega datos)
        {
            var resultado = new Respuesta<TBodega>();
            try
            {
                // Se busca la bodega actual en la base de datos por su Id.
                var actual = await _unidadDeTrabajo.TBodega.ObtenerEntidadAsync(x => x.BodegaId == datos.BodegaId);

                // Si no existe, se devuelve un error y se corta la ejecución.
                if (actual.Data == null)
                {
                    resultado.Error = "No existe la bodega a modificar.";
                    return resultado;
                }

                // Se actualiza la fecha de modificación en UTC.
                datos.ActualizadoEn = DateTime.UtcNow;

                // Se copian (mapean) los valores nuevos de "datos" sobre la entidad "actual"
                // que ya está siendo rastreada por el contexto de base de datos.
                _mapper.Map(datos, actual.Data);

                // Se guarda la entidad ya modificada.
                var respuesta = await _unidadDeTrabajo.TBodega.ModificarAsync(actual.Data);

                // Se confirman los cambios en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se mapea la entidad actualizada de vuelta al DTO de salida.
                resultado.Data = _mapper.Map<TBodega>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar BodegaId {BodegaId}", datos.BodegaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Elimina una bodega existente.
        public async Task<Respuesta<bool>> EliminarAsync(TBodega datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Se busca la bodega a eliminar por su Id.
                var bodega = await _unidadDeTrabajo.TBodega.ObtenerEntidadAsync(x => x.BodegaId == datos.BodegaId);

                // Si no existe, se informa el error y se detiene el proceso.
                if (bodega.Data == null)
                {
                    resultado.Error = "No existe la bodega a eliminar.";
                    return resultado;
                }

                // Se elimina la entidad encontrada.
                // Nota: a diferencia de CategoriaLN, aquí no se valida si la bodega tiene
                // registros dependientes (por ejemplo, existencias/inventario) antes de eliminar.
                var respuesta = await _unidadDeTrabajo.TBodega.EliminarAsync(bodega.Data);

                // Se confirma (commit) la eliminación en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se guarda el resultado booleano indicando si la eliminación fue exitosa.
                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar BodegaId {BodegaId}", datos.BodegaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Lista todas las bodegas registradas.
        public async Task<Respuesta<IEnumerable<TBodega>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TBodega>>();
            try
            {
                // Se obtienen todas las bodegas desde el repositorio.
                var resp = await _unidadDeTrabajo.TBodega.ListarAsync();

                // Se mapea la colección de entidades a una colección de DTOs.
                resultado.Data = _mapper.Map<IEnumerable<TBodega>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar bodegas.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Busca bodegas cuyo nombre contenga el texto indicado en "datos.Nombre".
        // Nota: no hay protección contra datos.Nombre nulo; si llega null, Contains
        // lanzaría una excepción que quedaría capturada por el catch.
        public async Task<Respuesta<IEnumerable<TBodega>>> BuscarAsync(TBodega datos)
        {
            var resultado = new Respuesta<IEnumerable<TBodega>>();
            try
            {
                // Filtro: nombre que contenga el texto buscado.
                var respuesta = await _unidadDeTrabajo.TBodega.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));

                // Se mapea el resultado a una colección de DTOs.
                resultado.Data = _mapper.Map<IEnumerable<TBodega>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar bodegas.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Obtiene una única bodega por su Id.
        public async Task<Respuesta<TBodega>> ObtenerAsync(TBodega datos)
        {
            var resultado = new Respuesta<TBodega>();
            try
            {
                // Se busca la entidad por BodegaId.
                var respuesta = await _unidadDeTrabajo.TBodega.ObtenerEntidadAsync(x => x.BodegaId == datos.BodegaId);

                // Si no se encuentra, se retorna un mensaje de error.
                if (respuesta.Data == null)
                {
                    resultado.Error = "Bodega no encontrada.";
                    return resultado;
                }

                // Se mapea la entidad encontrada al DTO de salida.
                resultado.Data = _mapper.Map<TBodega>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener BodegaId {BodegaId}", datos.BodegaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}