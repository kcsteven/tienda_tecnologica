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
    public class SubcategoriaLN : ISubcategoriaLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<SubcategoriaLN> _logger { get; }
        private readonly IMapper _mapper;

        public SubcategoriaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<SubcategoriaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TSubcategoria>> InsertarAsync(TSubcategoria datos)
        {
            var resultado = new Respuesta<TSubcategoria>();
            try
            {
                var categoria = await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(x => x.CategoriaId == datos.CategoriaId);
                if (categoria.Data == null)
                {
                    resultado.Error = "La categoría indicada no existe.";
                    return resultado;
                }

                datos.CreadoEn = DateTime.UtcNow;
                var entidad = _mapper.Map<Subcategoria>(datos);
                var respuesta = await _unidadDeTrabajo.TSubcategoria.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TSubcategoria>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar subcategoría {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TSubcategoria>> ModificarAsync(TSubcategoria datos)
        {
            var resultado = new Respuesta<TSubcategoria>();
            try
            {
                var actual = await _unidadDeTrabajo.TSubcategoria.ObtenerEntidadAsync(x => x.SubcategoriaId == datos.SubcategoriaId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe la subcategoría a modificar.";
                    return resultado;
                }

                datos.ActualizadoEn = DateTime.UtcNow;
                _mapper.Map(datos, actual.Data);

                var respuesta = await _unidadDeTrabajo.TSubcategoria.ModificarAsync(actual.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TSubcategoria>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar SubcategoriaId {SubcategoriaId}", datos.SubcategoriaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TSubcategoria datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var subcategoria = await _unidadDeTrabajo.TSubcategoria.ObtenerEntidadAsync(x => x.SubcategoriaId == datos.SubcategoriaId);
                if (subcategoria.Data == null)
                {
                    resultado.Error = "No existe la subcategoría a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TSubcategoria.EliminarAsync(subcategoria.Data);
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

        public async Task<Respuesta<IEnumerable<TSubcategoria>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TSubcategoria>>();
            try
            {
                var resp = await _unidadDeTrabajo.TSubcategoria.ListarAsync();
                if (!string.IsNullOrEmpty(resp.Error))
                {
                    resultado.Error = resp.Error;
                    return resultado;
                }
                resultado.Data = _mapper.Map<IEnumerable<TSubcategoria>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar subcategorías.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TSubcategoria>>> BuscarAsync(TSubcategoria datos)
        {
            var resultado = new Respuesta<IEnumerable<TSubcategoria>>();
            try
            {
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

        public async Task<Respuesta<TSubcategoria>> ObtenerAsync(TSubcategoria datos)
        {
            var resultado = new Respuesta<TSubcategoria>();
            try
            {
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

        public async Task<Respuesta<IEnumerable<TSubcategoria>>> ListarPorCategoriaAsync(int categoriaId)
        {
            var resultado = new Respuesta<IEnumerable<TSubcategoria>>();
            try
            {
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