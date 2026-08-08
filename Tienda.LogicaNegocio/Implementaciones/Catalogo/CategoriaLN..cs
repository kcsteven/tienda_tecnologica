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
    // Implementación de la lógica de negocio para la entidad "Categoria".
    // Implementa la interfaz ICategoriaLN, que define el contrato de operaciones disponibles.
    public class CategoriaLN : ICategoriaLN
    {
        // Unidad de trabajo (patrón Unit of Work) que agrupa los repositorios de acceso a datos
        // y permite controlar transacciones (guardar varios cambios de forma atómica).
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }

        // Logger para registrar errores u otra información relevante durante la ejecución.
        private ILogger<CategoriaLN> _logger { get; }

        // AutoMapper: se usa para convertir entre la entidad de base de datos (Categoria)
        // y el DTO/modelo tipado usado en la capa de negocio (TCategoria).
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias por inyección de dependencias (DI).
        public CategoriaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<CategoriaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta una nueva categoría en la base de datos.
        public async Task<Respuesta<TCategoria>> InsertarAsync(TCategoria datos)
        {
            var resultado = new Respuesta<TCategoria>();
            try
            {
                // Validación previa: se comprueba si ya existe una categoría con el mismo nombre,
                // para evitar duplicados.
                var existente = await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(x => x.Nombre == datos.Nombre);
                if (existente.Data != null)
                {
                    resultado.Error = "Ya existe una categoría registrada con ese nombre.";
                    return resultado;
                }

                // Se asigna la fecha de creación en UTC antes de guardar.
                datos.CreadoEn = DateTime.UtcNow;

                // Se mapea el DTO (TCategoria) a la entidad de base de datos (Categoria).
                var entidad = _mapper.Map<Categoria>(datos);

                // Se inserta la entidad usando el repositorio correspondiente dentro de la unidad de trabajo.
                var respuesta = await _unidadDeTrabajo.TCategoria.InsertarAsync(entidad);

                // Se confirma (commit) el cambio en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se mapea la entidad insertada de vuelta al DTO para devolverla al llamador.
                resultado.Data = _mapper.Map<TCategoria>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Se registra el error en el log.
                _logger.LogError(ex, "Error al insertar categoría {Nombre}", datos.Nombre);

                // Nota: a diferencia del resto de métodos/clases, aquí se prioriza el mensaje
                // de la InnerException (si existe) sobre el mensaje general de la excepción.
                // Esto suele hacerse porque, por ejemplo, errores de base de datos (violaciones
                // de restricciones, etc.) traen el detalle útil en la InnerException.
                resultado.Error = ex.InnerException?.Message ?? ex.Message;
            }
            return resultado;
        }

        // Modifica (actualiza) una categoría existente.
        public async Task<Respuesta<TCategoria>> ModificarAsync(TCategoria datos)
        {
            var resultado = new Respuesta<TCategoria>();
            try
            {
                // Se busca la categoría actual en la base de datos por su Id.
                var actual = await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(x => x.CategoriaId == datos.CategoriaId);

                // Si no existe, se devuelve un error y se corta la ejecución.
                if (actual.Data == null)
                {
                    resultado.Error = "No existe la categoría a modificar.";
                    return resultado;
                }

                // Se actualiza la fecha de modificación en UTC.
                datos.ActualizadoEn = DateTime.UtcNow;

                // Se copian (mapean) los valores nuevos de "datos" sobre la entidad "actual"
                // que ya está siendo rastreada por el contexto de base de datos.
                _mapper.Map(datos, actual.Data);

                // Se guarda la entidad ya modificada.
                var respuesta = await _unidadDeTrabajo.TCategoria.ModificarAsync(actual.Data);

                // Se confirman los cambios en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se mapea la entidad actualizada de vuelta al DTO de salida.
                resultado.Data = _mapper.Map<TCategoria>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar CategoriaId {CategoriaId}", datos.CategoriaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Elimina una categoría existente.
        public async Task<Respuesta<bool>> EliminarAsync(TCategoria datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Se busca la categoría a eliminar por su Id.
                var categoria = await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(x => x.CategoriaId == datos.CategoriaId);

                // Si no existe, se informa el error y se detiene el proceso.
                if (categoria.Data == null)
                {
                    resultado.Error = "No existe la categoría a eliminar.";
                    return resultado;
                }

                // Regla de negocio adicional (no presente en las otras clases revisadas):
                // antes de eliminar, se comprueba si existen subcategorías asociadas a esta categoría.
                var tieneSubcategorias = await _unidadDeTrabajo.TSubcategoria.BuscarAsync(x => x.CategoriaId == datos.CategoriaId);

                // Si la búsqueda devuelve datos y la lista tiene al menos un elemento,
                // se impide la eliminación para no dejar subcategorías huérfanas
                // (se protege la integridad referencial a nivel de negocio).
                if (tieneSubcategorias.Data != null && ((List<Subcategoria>)tieneSubcategorias.Data).Count > 0)
                {
                    resultado.Error = "No se puede eliminar: la categoría tiene subcategorías asociadas.";
                    return resultado;
                }

                // Se elimina la entidad encontrada.
                var respuesta = await _unidadDeTrabajo.TCategoria.EliminarAsync(categoria.Data);

                // Se confirma (commit) la eliminación en la base de datos.
                _unidadDeTrabajo.Completar();

                // Se guarda el resultado booleano indicando si la eliminación fue exitosa.
                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar CategoriaId {CategoriaId}", datos.CategoriaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Lista todas las categorías registradas.
        public async Task<Respuesta<IEnumerable<TCategoria>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TCategoria>>();
            try
            {
                // Se obtienen todas las categorías desde el repositorio.
                var resp = await _unidadDeTrabajo.TCategoria.ListarAsync();

                // Se mapea la colección de entidades a una colección de DTOs.
                resultado.Data = _mapper.Map<IEnumerable<TCategoria>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar categorías.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Busca categorías cuyo nombre contenga el texto indicado en "datos.Nombre".
        // Nota: no hay protección contra datos.Nombre nulo; si llega null, Contains
        // lanzaría una excepción que quedaría capturada por el catch.
        public async Task<Respuesta<IEnumerable<TCategoria>>> BuscarAsync(TCategoria datos)
        {
            var resultado = new Respuesta<IEnumerable<TCategoria>>();
            try
            {
                // Filtro: nombre que contenga el texto buscado.
                var respuesta = await _unidadDeTrabajo.TCategoria.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));

                // Se mapea el resultado a una colección de DTOs.
                resultado.Data = _mapper.Map<IEnumerable<TCategoria>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar categorías.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Obtiene una única categoría por su Id.
        public async Task<Respuesta<TCategoria>> ObtenerAsync(TCategoria datos)
        {
            var resultado = new Respuesta<TCategoria>();
            try
            {
                // Se busca la entidad por CategoriaId.
                var respuesta = await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(x => x.CategoriaId == datos.CategoriaId);

                // Si no se encuentra, se retorna un mensaje de error.
                if (respuesta.Data == null)
                {
                    resultado.Error = "Categoría no encontrada.";
                    return resultado;
                }

                // Se mapea la entidad encontrada al DTO de salida.
                resultado.Data = _mapper.Map<TCategoria>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener CategoriaId {CategoriaId}", datos.CategoriaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}