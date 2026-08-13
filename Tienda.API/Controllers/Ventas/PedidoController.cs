using Microsoft.AspNetCore.Mvc;
using System;
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

                // Intentamos obtener el correo y el nombre desde las propiedades de 'datos'
                // Usa reflexiones para leer Correo/Nombre dinámicamente si no están explícitos
                string correoCliente = datos.GetType().GetProperty("Correo")?.GetValue(datos)?.ToString()
                                    ?? datos.GetType().GetProperty("CorreoCliente")?.GetValue(datos)?.ToString()
                                    ?? "";

                string nombreCliente = datos.GetType().GetProperty("Nombre")?.GetValue(datos)?.ToString()
                                    ?? datos.GetType().GetProperty("NombreCliente")?.GetValue(datos)?.ToString()
                                    ?? "Cliente";

                if (pedidoGuardado != null && !string.IsNullOrEmpty(correoCliente))
                {
                    decimal total = pedidoGuardado.Total ?? 0m;
                    decimal subtotal = Math.Round(total / 1.13m, 2);
                    decimal iva = Math.Round(total - subtotal, 2);

                    await _correoService.EnviarFacturaAsync(
                        correoCliente,
                        nombreCliente,
                        pedidoGuardado.PedidoId,
                        subtotal,
                        iva,
                        total
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