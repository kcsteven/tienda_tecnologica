using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tienda.API.Servicios.Correo;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoLN _pedidoLN;
        private readonly ICorreoService _correoService;

        public PedidoController(IPedidoLN pedidoLN, ICorreoService correoService)
        {
            _pedidoLN = pedidoLN;
            _correoService = correoService;
        }

        [HttpGet("Listar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Listar()
        {
            var resultado = await _pedidoLN.ListarAsync();
            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpGet("Obtener/{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Obtener(int id)
        {
            var resultado = await _pedidoLN.ObtenerAsync(
                new TPedido
                {
                    PedidoId = id
                });

            if (!string.IsNullOrEmpty(resultado.Error))
                return NotFound(resultado);

            return Ok(resultado);
        }

        [HttpGet("Buscar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(string nombrePedido)
        {
            var resultado = await _pedidoLN.BuscarAsync(
                new TPedido
                {
                    NombrePedido = nombrePedido
                });

            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TPedido pedido)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _pedidoLN.InsertarAsync(pedido);
            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TPedido pedido)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _pedidoLN.ModificarAsync(pedido);
            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _pedidoLN.EliminarAsync(
                new TPedido
                {
                    PedidoId = id
                });

            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // POST: api/Pedido/CrearCompra
        [HttpPost("CrearCompra")]
        public async Task<IActionResult> CrearCompra([FromBody] TPedidoCrear datos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Le pasamos 'datos' (que es de tipo TPedidoCrear) directamente al método de la capa de negocio
            var resultado = await _pedidoLN.CrearCompraAsync(datos);

            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            // 2. Intentar enviar la factura por correo de manera segura
            try
            {
                var pedidoGuardado = resultado.Data;

                // Antes intentaba leer Correo/Nombre por reflexión desde 'datos' (TPedidoCrear),
                // pero ese DTO nunca trae esos campos (solo clienteId, direccionId, metodoPagoId, detalles),
                // por eso el correo nunca se enviaba.
                // Ahora CrearCompraAsync ya busca el cliente en la BD y devuelve CorreoCliente/NombreCliente
                // dentro del propio 'resultado.Data' (TPedido), así que los tomo directo de ahí.
                string correoCliente = pedidoGuardado?.CorreoCliente ?? "";
                string nombreCliente = pedidoGuardado?.NombreCliente ?? "Cliente";

                // (Ya quité el Console.WriteLine "[DEBUG]" que tenía aquí, era solo
                // para diagnosticar por qué no llegaba el correo. Ya se confirmó que funciona.)

                if (pedidoGuardado != null && !string.IsNullOrEmpty(correoCliente))
                {
                    decimal total = pedidoGuardado.Total ?? 0m;
                    decimal subtotal = Math.Round(total / 1.13m, 2);
                    decimal iva = Math.Round(total - subtotal, 2);

                    // Lista de productos comprados (nombre, cantidad, precio, imagen) que
                    // CrearCompraAsync ya armó y dejó en resultado.Data.ItemsFactura.
                    // Ya NO convierto ImagenUrl a URL absoluta aquí: ahora CorreoService
                    // incrusta la imagen directo desde el archivo físico en wwwroot (cid:),
                    // así que necesita la ruta relativa tal cual viene, no una URL.
                    var items = pedidoGuardado.ItemsFactura ?? new List<TItemFactura>();

                    await _correoService.EnviarFacturaAsync(
                        correoCliente,
                        nombreCliente,
                        pedidoGuardado.PedidoId,
                        subtotal,
                        iva,
                        total,
                        items
                    );
                }
            }
            catch (Exception ex)
            {
                // Si el envío de correo falla, la transacción en base de datos no se ve afectada
                Console.WriteLine($"[AVISO] Compra realizada con éxito pero falló el envío de correo: {ex.Message}");
            }

            return Ok(resultado);
        }
    }
}