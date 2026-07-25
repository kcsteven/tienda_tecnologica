
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
    public class ListaDeseosLN : IListaDeseosLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        private ILogger<ListaDeseosLN> _logger { get; }
        private readonly IMapper _mapper;

        public ListaDeseosLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ListaDeseosLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TListaDeseos>> InsertarAsync(TListaDeseos datos)
        {
            var resultado = new Respuesta<TListaDeseos>();
            try
            {
                var cliente = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(x => x.ClienteId == datos.ClienteId);
                if (cliente.Data == null)
                {
                    resultado.Error = "El cliente indicado no existe.";
                    return resultado;
                }

                var producto = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(x => x.ProductoId == datos.ProductoId);
                if (producto.Data == null)
                {
                    resultado.Error = "El producto indicado no existe.";
                    return resultado;
                }

                datos.FechaAgregado = DateTime.UtcNow;
                var entidad = _mapper.Map<ListaDeseos>(datos);
                var respuesta = await _unidadDeTrabajo.TListaDeseos.InsertarAsync(entidad);
                _unidadDeTrabajo.Completar();

                resultado.Data = _mapper.Map<TListaDeseos>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar en lista de deseos {ProductoId}", datos.ProductoId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TListaDeseos datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                var registro = await _unidadDeTrabajo.TListaDeseos.ObtenerEntidadAsync(x => x.ListaDeseosId == datos.ListaDeseosId);
                if (registro.Data == null)
                {
                    resultado.Error = "No existe el registro a eliminar.";
                    return resultado;
                }

                var respuesta = await _unidadDeTrabajo.TListaDeseos.EliminarAsync(registro.Data);
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ListaDeseosId {Id}", datos.ListaDeseosId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TListaDeseos>>> ListarPorClienteAsync(int clienteId)
        {
            var resultado = new Respuesta<IEnumerable<TListaDeseos>>();
            try
            {
                var respuesta = await _unidadDeTrabajo.TListaDeseos.BuscarAsync(x => x.ClienteId == clienteId);
                resultado.Data = _mapper.Map<IEnumerable<TListaDeseos>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar deseos del cliente {ClienteId}", clienteId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}