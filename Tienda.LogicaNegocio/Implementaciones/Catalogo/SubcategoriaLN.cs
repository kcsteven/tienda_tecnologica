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
    // Implementación de la lógica de negocio (LN) para la entidad Subcategoria
    public class SubcategoriaLN : ISubcategoriaLN
    {
        // Unidad de trabajo (Entity Framework) para acceder a los repositorios de datos
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        // Logger para registrar errores y eventos de esta clase
        private ILogger<SubcategoriaLN> _logger { get; }
        // AutoMapper para convertir entre entidades de dominio (Subcategoria) y entidades tipadas (TSubcategoria)
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias mediante inyección de dependencias
        public SubcategoriaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<SubcategoriaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta una nueva subcategoría, validando primero que la categoría padre exista
        public async Task<Respuesta<TSubcategoria>> InsertarAsync(TSubcategoria datos)
        {
            var resultado = new Respuesta<TSubcategoria>();
            try
            {
                // Verifica que la categoría a la que pertenece la subcategoría exista
                var categoria = await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(x => x.CategoriaId == datos.CategoriaId);
                if (categoria.Data == null)
                {
                    resultado.Error = "La categoría indicada no existe.";
                    return resultado;
                }

                // Marca la fecha de creación en UTC
                datos.CreadoEn = DateTime.UtcNow;
                // Convierte el DTO tipado a la entidad de dominio
                var entidad = _mapper.Map<Subcategoria>(datos);
                // Inserta la entidad en el repositorio
                var respuesta = await _unidadDeTrabajo.TSubcategoria.InsertarAsync(entidad);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                // Convierte la entidad insertada de vuelta a DTO tipado para la respuesta
                resultado.Data = _mapper.Map<TSubcategoria>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Registra el error y lo devuelve en la respuesta
                _logger.LogError(ex, "Error al insertar subcategoría {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Modifica una subcategoría existente, validando primero que exista
        public async Task<Respuesta<TSubcategoria>> ModificarAsync(TSubcategoria datos)
        {
            var resultado = new Respuesta<TSubcategoria>();
            try
            {
                // Busca la subcategoría actual en base de datos por su Id
                var actual = await _unidadDeTrabajo.TSubcategoria.ObtenerEntidadAsync(x => x.SubcategoriaId == datos.SubcategoriaId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe la subcategoría a modificar.";
                    return resultado;
                }

                // Marca la fecha de actualización en UTC
                datos.ActualizadoEn = DateTime.UtcNow;
                // Copia los valores del DTO recibido sobre la entidad existente rastreada por EF
                _mapper.Map(datos, actual.Data);

                // Guarda los cambios en el repositorio
                var respuesta = await _unidadDeTrabajo.TSubcategoria.ModificarAsync(actual.Data);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                // Convierte la entidad modificada de vuelta a DTO tipado para la respuesta
                resultado.Data = _mapper.Map<TSubcategoria>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar SubcategoriaId {SubcategoriaId}", datos.SubcategoriaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Elimina una subcategoría existente, validando primero que exista
        public async Task<Respuesta<bool>> EliminarAsync(TSubcategoria datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Busca la subcategoría a eliminar por su Id
                var subcategoria = await _unidadDeTrabajo.TSubcategoria.ObtenerEntidadAsync(x => x.SubcategoriaId == datos.SubcategoriaId);
                if (subcategoria.Data == null)
                {
                    resultado.Error = "No existe la subcategoría a eliminar.";
                    return resultado;
                }

                // Elimina la entidad del repositorio
                var respuesta = await _unidadDeTrabajo.TSubcategoria.EliminarAsync(subcategoria.Data);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar SubcategoriaId {SubcategoriaId}", datos.SubcategoriaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Lista todas las subcategorías existentes
        public async Task<Respuesta<IEnumerable<TSubcategoria>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TSubcategoria>>();
            try
            {
                // Obtiene todas las subcategorías desde el repositorio
                var resp = await _unidadDeTrabajo.TSubcategoria.ListarAsync();
                if (!string.IsNullOrEmpty(resp.Error))
                {
                    resultado.Error = resp.Error;
                    return resultado;
                }
                // Convierte la lista de entidades de dominio a DTOs tipados
                resultado.Data = _mapper.Map<IEnumerable<TSubcategoria>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar subcategorías.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Busca subcategorías cuyo nombre contenga el texto recibido
        public async Task<Respuesta<IEnumerable<TSubcategoria>>> BuscarAsync(TSubcategoria datos)
        {
            var resultado = new Respuesta<IEnumerable<TSubcategoria>>();
            try
            {
                // Filtra las subcategorías cuyo nombre contenga el texto de búsqueda
                var respuesta = await _unidadDeTrabajo.TSubcategoria.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));
                if (!string.IsNullOrEmpty(respuesta.Error))
                {
                    resultado.Error = respuesta.Error;
                    return resultado;
                }
                resultado.Data = _mapper.Map<IEnumerable<TSubcategoria>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar subcategorías.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Obtiene una subcategoría puntual según su Id
        public async Task<Respuesta<TSubcategoria>> ObtenerAsync(TSubcategoria datos)
        {
            var resultado = new Respuesta<TSubcategoria>();
            try
            {
                // Busca la subcategoría por su Id
                var respuesta = await _unidadDeTrabajo.TSubcategoria.ObtenerEntidadAsync(x => x.SubcategoriaId == datos.SubcategoriaId);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Subcategoría no encontrada.";
                    return resultado;
                }
                resultado.Data = _mapper.Map<TSubcategoria>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener SubcategoriaId {SubcategoriaId}", datos.SubcategoriaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Lista las subcategorías que pertenecen a una categoría específica
        public async Task<Respuesta<IEnumerable<TSubcategoria>>> ListarPorCategoriaAsync(int categoriaId)
        {
            var resultado = new Respuesta<IEnumerable<TSubcategoria>>();
            try
            {
                // Filtra las subcategorías por el Id de categoría recibido
                var respuesta = await _unidadDeTrabajo.TSubcategoria.BuscarAsync(x => x.CategoriaId == categoriaId);
                if (!string.IsNullOrEmpty(respuesta.Error))
                {
                    resultado.Error = respuesta.Error;
                    return resultado;
                }
                resultado.Data = _mapper.Map<IEnumerable<TSubcategoria>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar subcategorías de CategoriaId {CategoriaId}", categoriaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}