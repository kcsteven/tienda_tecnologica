
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvioController : ControllerBase
    {
        private IEnvioLN _envioLN { get; }
        public EnvioController(IEnvioLN envioLN) { _envioLN = envioLN; }

        [HttpGet("ObtenerPorPedido/{pedidoId}")]
        public async Task<IActionResult> ObtenerPorPedido(int pedidoId)
        {
            var resultado = await _envioLN.ObtenerPorPedidoAsync(pedidoId);
            if (!string.IsNullOrEmpty(resultado.Error)) return NotFound(resultado);
            return Ok(resultado);
        }

        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TEnvio datos)
        {
            var resultado = await _envioLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TEnvio datos)
        {
            var resultado = await _envioLN.ModificarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}