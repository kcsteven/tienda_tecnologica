
using Microsoft.AspNetCore.Mvc;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Dominio.InterfazLN;

namespace Ventas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        private IPagoLN _pagoLN { get; }
        public PagoController(IPagoLN pagoLN) { _pagoLN = pagoLN; }

        [HttpGet("ListarPorPedido/{pedidoId}")]
        public async Task<IActionResult> ListarPorPedido(int pedidoId)
        {
            var resultado = await _pagoLN.ListarPorPedidoAsync(pedidoId);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TPago datos)
        {
            var resultado = await _pagoLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _pagoLN.EliminarAsync(new TPago { PagoId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}