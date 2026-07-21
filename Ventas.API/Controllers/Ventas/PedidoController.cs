using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Dominio.InterfazLN;

namespace Ventas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private IPedidoLN _pedidoLN { get; }

        public PedidoController(IPedidoLN pedidoLN)
        {
            _pedidoLN = pedidoLN;
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

    }
}
