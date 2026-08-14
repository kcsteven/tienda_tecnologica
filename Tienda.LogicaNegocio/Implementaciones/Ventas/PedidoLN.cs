using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;
using System.Data;

namespace Tienda.LogicaNegocio.Implementaciones
{
    public class PedidoLN : IPedidoLN
    {
        private IUnidadTrabajoEF _unidadDeTrabajo { set; get; }
        private ILogger<PedidoLN> _logger { get; }
        private readonly IMapper _mapper;

        public PedidoLN(
            IUnidadTrabajoEF unidadTrabajo,
            ILogger<PedidoLN> logger,
            IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Respuesta<TPedido>> InsertarAsync(TPedido datos)
        {
            var resultado = new Respuesta<TPedido>();

            try
            {
                var entidad = _mapper.Map<Pedido>(datos);

                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.InsertarAsync(entidad);

                _unidadDeTrabajo.Completar();

                resultado.Data =
                    _mapper.Map<TPedido>(respuestaRepositorio.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar pedido.");
                resultado.Error = ex.Message;
            }

            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TPedido>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TPedido>>();

            try
            {
                var resp = await _unidadDeTrabajo.TPedido.ListarAsync();

                resultado.Data =
                    _mapper.Map<IEnumerable<TPedido>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar pedidos.");
                resultado.Error = ex.Message;
            }

            return resultado;
        }

        public async Task<Respuesta<TPedido>> ModificarAsync(TPedido datos)
        {
            var resultado = new Respuesta<TPedido>();

            try
            {
                var pedidoActual =
                    await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(
                        x => x.PedidoId == datos.PedidoId);

                if (pedidoActual.Data == null)
                {
                    resultado.Error = "No existe el pedido a modificar.";
                    return resultado;
                }

                _mapper.Map(datos, pedidoActual.Data);

                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.ModificarAsync(
                        pedidoActual.Data);

                _unidadDeTrabajo.Completar();

                resultado.Data =
                    _mapper.Map<TPedido>(respuestaRepositorio.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al modificar PedidoId {PedidoId}",
                    datos.PedidoId);

                resultado.Error = ex.Message;
            }

            return resultado;
        }

        public async Task<Respuesta<bool>> EliminarAsync(TPedido datos)
        {
            var resultado = new Respuesta<bool>();

            try
            {
                var pedido =
                    await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(
                        x => x.PedidoId == datos.PedidoId);

                if (pedido.Data == null)
                {
                    resultado.Error = "No existe el pedido a eliminar.";
                    return resultado;
                }

                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.EliminarAsync(pedido.Data);

                _unidadDeTrabajo.Completar();

                resultado.Data = respuestaRepositorio.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al eliminar PedidoId {PedidoId}",
                    datos.PedidoId);

                resultado.Error = ex.Message;
            }

            return resultado;
        }

        public async Task<Respuesta<IEnumerable<TPedido>>> BuscarAsync(TPedido datos)
        {
            var resultado = new Respuesta<IEnumerable<TPedido>>();

            try
            {
                // Búsqueda por cliente
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.BuscarAsync(
                        x => x.ClienteId == datos.ClienteId);

                resultado.Data =
                    _mapper.Map<IEnumerable<TPedido>>(respuestaRepositorio.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar pedidos.");
                resultado.Error = ex.Message;
            }

            return resultado;
        }

        public async Task<Respuesta<TPedido>> ObtenerAsync(TPedido datos)
        {
            var resultado = new Respuesta<TPedido>();

            try
            {
                var respuestaRepositorio =
                    await _unidadDeTrabajo.TPedido.ObtenerEntidadAsync(
                        x => x.PedidoId == datos.PedidoId);

                if (respuestaRepositorio.Data == null)
                {
                    resultado.Error = "Pedido no encontrado.";
                    return resultado;
                }

                resultado.Data =
                    _mapper.Map<TPedido>(respuestaRepositorio.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al obtener PedidoId {PedidoId}",
                    datos.PedidoId);

                resultado.Error = ex.Message;
            }

            return resultado;
        }

        public async Task<Respuesta<TPedido>> CrearCompraAsync(TPedidoCrear datos)
        {
            var resultado = new Respuesta<TPedido>();
            var transaccionActiva = false;

            void RevertirTransaccion()
            {
                if (!transaccionActiva)
                {
                    return;
                }

                try
                {
                    _unidadDeTrabajo.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(
                        rollbackEx,
                        "Error al revertir la compra");
                }

                finally
                {
                    transaccionActiva = false;
                }
            }

            try
            {
                // 1. Buscar el estado "Pendiente" por nombre (no asumimos que su Id sea 1,
                // porque el orden de inserción de los estados puede variar según el entorno)
                var resEstadoPendiente = await _unidadDeTrabajo.TEstadoPedido.ObtenerEntidadAsync(
                    e => e.Nombre == "Pendiente");

                if (!string.IsNullOrEmpty(resEstadoPendiente.Error) || resEstadoPendiente.Data == null)
                {
                    _logger.LogError(
                        "No se encontró el EstadoPedido 'Pendiente' en CrearCompraAsync: {Error}",
                        resEstadoPendiente.Error);

                    resultado.Error = "No se pudo crear el pedido: no existe el estado 'Pendiente' configurado en la base de datos.";
                    return resultado;
                }

                _unidadDeTrabajo.EmpezarTransaccion(
                    IsolationLevel.Serializable);

                transaccionActiva = true;

                // 2. Crear el Pedido
                var nuevoPedido = new Pedido
                {
                    ClienteId = datos.ClienteId,
                    DireccionId = datos.DireccionId,
                    EstadoPedidoId = resEstadoPendiente.Data.EstadoPedidoId,
                    FechaPedido = DateTime.Now,
                    Total = 0
                };

                var resPedido = await _unidadDeTrabajo.TPedido.InsertarAsync(nuevoPedido);

                // Si el INSERT del pedido falló (ej. restricción de la base de datos),
                // detenemos aquí y devolvemos el error real en vez de seguir con datos nulos.
                if (!string.IsNullOrEmpty(resPedido.Error) || resPedido.Data == null)
                {
                    _logger.LogError(
                        "Error al insertar el Pedido en CrearCompraAsync: {Error}",
                        resPedido.Error);

                    resultado.Error = resPedido.Error ?? "No fue posible crear el pedido.";
                    RevertirTransaccion();
                    return resultado;
                }

                decimal totalAcumulado = 0;

                // Voy guardando nombre + imagen de cada producto comprado
                // para poder armar la factura con esos datos más adelante
                var itemsFactura = new List<TItemFactura>();

                // 3. Crear los detalles del pedido
                foreach (var item in datos.Detalles)
                {
                    // Incluyo ImagenProductos porque, sin el include, EF no la trae
                    // y necesito la ruta de la imagen para la factura
                    var prodRes = await _unidadDeTrabajo.TProducto.ObtenerEntidadAsync(
                        p => p.ProductoId == item.ProductoId,
                        new List<string> { "ImagenProductos" });

                    if (!string.IsNullOrEmpty(prodRes.Error) || prodRes.Data == null || !prodRes.Data.Activo)
                    {
                        resultado.Error = "Uno de los productos no existe o no esta disponible";

                        RevertirTransaccion();
                        return resultado;
                    }

                    var resInventario = await _unidadDeTrabajo.TInventario.BuscarAsync(
                        inventario => inventario.ProductoId == item.ProductoId &&
                        inventario.Cantidad > 0, new List<string> { "Bodega" });

                    if (!string.IsNullOrEmpty(resInventario.Error))
                    {
                        resultado.Error = "No fue posible consultar con el inventario";

                        RevertirTransaccion();
                        return resultado;
                    }

                    var inventarioDisponibles = (resInventario.Data ?? Enumerable.Empty<Inventario>())
                        .Where(inventario => inventario.Bodega != null &&
                        inventario.Bodega != null && inventario.Bodega.Activo && inventario.Cantidad > 0)
                        .OrderBy(inventario => inventario.BodegaId)
                        .ThenBy(inventario => inventario.InventarioId)
                        .ToList();

                    var cantidadDisponible = inventarioDisponibles.Sum(
                        inventario => (long)inventario.Cantidad);

                    if (item.Cantidad > cantidadDisponible)
                    {
                        resultado.Error = $"No hay suficiente stock para {prodRes.Data.Nombre} " +
                            $"Disponible: {cantidadDisponible}";

                        RevertirTransaccion();
                        return resultado;
                    }

                    var cantidadRestante = item.Cantidad;

                    foreach (var inventario in inventarioDisponibles)
                    {
                        if (cantidadRestante == 0)
                        {
                            break;
                        }

                        var cantidadADescontar = Math.Min(
                            inventario.Cantidad, cantidadRestante);

                        inventario.Cantidad -= cantidadADescontar;
                        cantidadRestante -= cantidadADescontar;

                        var resActualizarInventario = await _unidadDeTrabajo.TInventario.ModificarAsync(inventario);

                        if (!string.IsNullOrEmpty(
                            resActualizarInventario.Error))
                        {
                            resultado.Error = "No fue posible actualizar el inventario";

                            RevertirTransaccion();
                            return resultado;
                        }
                    }


                    var precio = prodRes.Data.Precio;
                    totalAcumulado += precio * item.Cantidad;

                    var detalle = new DetallesPedido
                    {
                        PedidoId = resPedido.Data.PedidoId,
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = precio
                    };

                    var resDetalle = await _unidadDeTrabajo.TDetallePedido.InsertarAsync(detalle);

                    if (!string.IsNullOrEmpty(resDetalle.Error))
                    {
                        _logger.LogError(
                            "Error al insertar DetallePedido en CrearCompraAsync: {Error}",
                            resDetalle.Error);

                        resultado.Error = resDetalle.Error;
                        RevertirTransaccion();
                        return resultado;
                    }

                    // Nombre + primera imagen del producto, para la factura por correo
                    itemsFactura.Add(new TItemFactura
                    {
                        Nombre = prodRes.Data.Nombre,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = precio,
                        ImagenUrl = prodRes.Data.ImagenProductos.FirstOrDefault()?.RutaImagen
                    });
                }

                // 4. Actualizar total con IVA (13%)
                resPedido.Data.Total = totalAcumulado * 1.13m;
                var resModificar = await _unidadDeTrabajo.TPedido.ModificarAsync(resPedido.Data);

                if (!string.IsNullOrEmpty(resModificar.Error))
                {
                    _logger.LogError(
                        "Error al actualizar el total del Pedido en CrearCompraAsync: {Error}",
                        resModificar.Error);

                    resultado.Error = resModificar.Error;
                    RevertirTransaccion() ;
                    return resultado;
                }

                // 5. Registrar Pago
                var pago = new Pago
                {
                    PedidoId = resPedido.Data.PedidoId,
                    MetodoPagoId = datos.MetodoPagoId,
                    Monto = resPedido.Data.Total,
                    FechaPago = DateTime.Now,
                    Referencia = "PAGO-WEB-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper()
                };

                var resPago = await _unidadDeTrabajo.TPago.InsertarAsync(pago);

                if (!string.IsNullOrEmpty(resPago.Error))
                {
                    _logger.LogError(
                        "Error al insertar el Pago en CrearCompraAsync: {Error}",
                        resPago.Error);

                    resultado.Error = resPago.Error;
                    RevertirTransaccion();
                    return resultado;
                }

                _unidadDeTrabajo.CompletarTran();
                transaccionActiva = false;
                resultado.Data = _mapper.Map<TPedido>(resPedido.Data);

                // Traer el correo/nombre del cliente para la factura
                var resCliente = await _unidadDeTrabajo.TCliente.ObtenerEntidadAsync(
                    c => c.ClienteId == datos.ClienteId,
                    new List<string> { "Persona" });

                if (resCliente.Data?.Persona != null)
                {
                    resultado.Data.CorreoCliente = resCliente.Data.Persona.Email;
                    resultado.Data.NombreCliente =
                        $"{resCliente.Data.Persona.Nombre} {resCliente.Data.Persona.Apellido}";
                }

                // Adjunto la lista de productos comprados (con imagen) al resultado,
                // para que el controller se la pase al servicio de correo
                resultado.Data.ItemsFactura = itemsFactura;
            }
            catch (Exception ex)
            {
                RevertirTransaccion();
                _logger.LogError(ex, "Error al procesar la compra completa.");
                resultado.Error = ex.Message;
            }

            return resultado;
        }
    }
}