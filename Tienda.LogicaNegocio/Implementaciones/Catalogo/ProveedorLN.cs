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
    public class ProveedorLN : IProveedorLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<ProveedorLN> _logger { get; }
        private readonly IMapper _mapper;

        public ProveedorLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ProveedorLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TProveedor>> InsertarAsync(TProveedor datos)
        {
            var resultado = new Respuesta<TProveedor>();
            try
            {
                var existente = await _unidadDeTrabajo.TProveedor.ObtenerEntidadAsync(x => x.Nombre == datos.Nombre);
                if (existente.Data != null)
                {
                    resultado.Error = "Ya existe un proveedor registrado con ese nombre.";
                    return resultado;
                }

                datos.CreadoEn = DateTime.UtcNow;
                var entidad = _mapper.Map<Proveedor>(datos);
                var respuesta = await _unidadDeTrabajo.TProveedor.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TProveedor>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar proveedor {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TProveedor>> ModificarAsync(TProveedor datos)
        {
            var resultado = new Respuesta<TProveedor>();
            try
            {
                var actual = await _unidadDeTrabajo.TProveedor.ObtenerEntidadAsync(x => x.ProveedorId == datos.ProveedorId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe el proveedor a modificar.";
                    return resultado;
                }

                datos.ActualizadoEn = DateTime.UtcNow;
                _mapper.Map(datos, actual.Data);

                var respuesta = await _unidadDeTrabajo.TProveedor.ModificarAsync(actual.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TProveedor>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar ProveedorId {ProveedorId}", datos.ProveedorId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TProveedor datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var proveedor = await _unidadDeTrabajo.TProveedor.ObtenerEntidadAsync(x => x.ProveedorId == datos.ProveedorId);
                if (proveedor.Data == null)
                {
                    resultado.Error = "No existe el proveedor a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TProveedor.EliminarAsync(proveedor.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ProveedorId {ProveedorId}", datos.ProveedorId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TProveedor>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TProveedor>>();
            try
            {
                var resp = await _unidadDeTrabajo.TProveedor.ListarAsync();
                resultado.Data = _mapper.Map<IEnumerable<TProveedor>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar proveedores.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TProveedor>>> BuscarAsync(TProveedor datos)
        {
            var resultado = new Respuesta<IEnumerable<TProveedor>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TProveedor.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));
                resultado.Data = _mapper.Map<IEnumerable<TProveedor>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar proveedores.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<TProveedor>> ObtenerAsync(TProveedor datos)
        {
            var resultado = new Respuesta<TProveedor>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TProveedor.ObtenerEntidadAsync(x => x.ProveedorId == datos.ProveedorId);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Proveedor no encontrado.";
                    return resultado;
                }
                resultado.Data = _mapper.Map<TProveedor>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ProveedorId {ProveedorId}", datos.ProveedorId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}