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
    public class CategoriaLN : ICategoriaLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<CategoriaLN> _logger { get; }
        private readonly IMapper _mapper;

        public CategoriaLN(IUnidadTrabajoEF unidadTrabajo, ILogger<CategoriaLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TCategoria>> InsertarAsync(TCategoria datos)
        {
            var resultado = new Respuesta<TCategoria>();
            try
            {
                var existente = await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(x => x.Nombre == datos.Nombre);
                if (existente.Data != null)
                {
                    resultado.Error = "Ya existe una categoría registrada con ese nombre.";
                    return resultado;
                }

                datos.CreadoEn = DateTime.UtcNow;
                var entidad = _mapper.Map<Categoria>(datos);
                var respuesta = await _unidadDeTrabajo.TCategoria.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TCategoria>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar categoría {Nombre}", datos.Nombre);
                resultado.Error = ex.InnerException?.Message ?? ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TCategoria>> ModificarAsync(TCategoria datos)
        {
            var resultado = new Respuesta<TCategoria>();
            try
            {
                var actual = await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(x => x.CategoriaId == datos.CategoriaId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe la categoría a modificar.";
                    return resultado;
                }

                datos.ActualizadoEn = DateTime.UtcNow;
                _mapper.Map(datos, actual.Data);

                var respuesta = await _unidadDeTrabajo.TCategoria.ModificarAsync(actual.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TCategoria>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar CategoriaId {CategoriaId}", datos.CategoriaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TCategoria datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var categoria = await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(x => x.CategoriaId == datos.CategoriaId);
                if (categoria.Data == null)
                {
                    resultado.Error = "No existe la categoría a eliminar.";
                    return resultado;
                }

                var tieneSubcategorias = await _unidadDeTrabajo.TSubcategoria.BuscarAsync(x => x.CategoriaId == datos.CategoriaId);
                if (tieneSubcategorias.Data != null && ((List<Subcategoria>)tieneSubcategorias.Data).Count > 0)
                {
                    resultado.Error = "No se puede eliminar: la categoría tiene subcategorías asociadas.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TCategoria.EliminarAsync(categoria.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar CategoriaId {CategoriaId}", datos.CategoriaId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TCategoria>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TCategoria>>();
            try
            {
                var resp = await _unidadDeTrabajo.TCategoria.ListarAsync();
                resultado.Data = _mapper.Map<IEnumerable<TCategoria>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar categorías.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TCategoria>>> BuscarAsync(TCategoria datos)
        {
            var resultado = new Respuesta<IEnumerable<TCategoria>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TCategoria.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));
                resultado.Data = _mapper.Map<IEnumerable<TCategoria>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar categorías.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TCategoria>> ObtenerAsync(TCategoria datos)
        {
            var resultado = new Respuesta<TCategoria>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TCategoria.ObtenerEntidadAsync(x => x.CategoriaId == datos.CategoriaId);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Categoría no encontrada.";
                    return resultado;
                }
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