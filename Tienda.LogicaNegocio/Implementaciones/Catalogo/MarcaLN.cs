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
    // Implementación de la lógica de negocio (LN) para la entidad Marca
    public class MarcaLN : IMarcaLN
    {
        // Unidad de trabajo (Entity Framework) para acceder a los repositorios de datos
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        // Logger para registrar errores y eventos de esta clase
        private ILogger<MarcaLN> _logger { get; }
        // AutoMapper para convertir entre entidades de dominio (Marca) y entidades tipadas (TMarca)
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias mediante inyección de dependencias
        public MarcaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<MarcaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta una nueva marca, validando primero que no exista ya una con el mismo nombre
        public async Task<Respuesta<TMarca>> InsertarAsync(TMarca datos)
        {
            var resultado = new Respuesta<TMarca>();
            try
            {
                // Verifica si ya existe una marca registrada con el mismo nombre
                var existente = await _unidadDeTrabajo.TMarca.ObtenerEntidadAsync(x => x.Nombre == datos.Nombre);
                if (existente.Data != null)
                {
                    resultado.Error = "Ya existe una marca registrada con ese nombre.";
                    return resultado;
                }

                // Marca la fecha de creación en UTC
                datos.CreadoEn = DateTime.UtcNow;
                // Convierte el DTO tipado a la entidad de dominio
                var entidad = _mapper.Map<Marca>(datos);
                // Inserta la entidad en el repositorio
                var respuesta = await _unidadDeTrabajo.TMarca.InsertarAsync(entidad);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                // Convierte la entidad insertada de vuelta a DTO tipado para la respuesta
                resultado.Data = _mapper.Map<TMarca>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Registra el error y lo devuelve en la respuesta
                _logger.LogError(ex, "Error al insertar marca {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Modifica una marca existente, validando primero que exista
        public async Task<Respuesta<TMarca>> ModificarAsync(TMarca datos)
        {
            var resultado = new Respuesta<TMarca>();
            try
            {
                // Busca la marca actual en base de datos por su Id
                var actual = await _unidadDeTrabajo.TMarca.ObtenerEntidadAsync(x => x.MarcaId == datos.MarcaId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe la marca a modificar.";
                    return resultado;
                }

                // Marca la fecha de actualización en UTC
                datos.ActualizadoEn = DateTime.UtcNow;
                // Copia los valores del DTO recibido sobre la entidad existente rastreada por EF
                _mapper.Map(datos, actual.Data);

                // Guarda los cambios en el repositorio
                var respuesta = await _unidadDeTrabajo.TMarca.ModificarAsync(actual.Data);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                // Convierte la entidad modificada de vuelta a DTO tipado para la respuesta
                resultado.Data = _mapper.Map<TMarca>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar MarcaId {MarcaId}", datos.MarcaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Elimina una marca existente, validando primero que exista
        public async Task<Respuesta<bool>> EliminarAsync(TMarca datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Busca la marca a eliminar por su Id
                var marca = await _unidadDeTrabajo.TMarca.ObtenerEntidadAsync(x => x.MarcaId == datos.MarcaId);
                if (marca.Data == null)
                {
                    resultado.Error = "No existe la marca a eliminar.";
                    return resultado;
                }

                // Elimina la entidad del repositorio
                var respuesta = await _unidadDeTrabajo.TMarca.EliminarAsync(marca.Data);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar MarcaId {MarcaId}", datos.MarcaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Lista todas las marcas existentes
        public async Task<Respuesta<IEnumerable<TMarca>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TMarca>>();
            try
            {
                // Obtiene todas las marcas desde el repositorio
                var resp = await _unidadDeTrabajo.TMarca.ListarAsync();
                // Convierte la lista de entidades de dominio a DTOs tipados
                resultado.Data = _mapper.Map<IEnumerable<TMarca>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar marcas.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Busca marcas cuyo nombre contenga el texto recibido
        public async Task<Respuesta<IEnumerable<TMarca>>> BuscarAsync(TMarca datos)
        {
            var resultado = new Respuesta<IEnumerable<TMarca>>();
            try
            {
                // Filtra las marcas cuyo nombre contenga el texto de búsqueda
                var respuesta = await _unidadDeTrabajo.TMarca.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));
                resultado.Data = _mapper.Map<IEnumerable<TMarca>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar marcas.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Obtiene una marca puntual según su Id
        public async Task<Respuesta<TMarca>> ObtenerAsync(TMarca datos)
        {
            var resultado = new Respuesta<TMarca>();
            try
            {
                // Busca la marca por su Id
                var respuesta = await _unidadDeTrabajo.TMarca.ObtenerEntidadAsync(x => x.MarcaId == datos.MarcaId);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Marca no encontrada.";
                    return resultado;
                }
                resultado.Data = _mapper.Map<TMarca>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener MarcaId {MarcaId}", datos.MarcaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}