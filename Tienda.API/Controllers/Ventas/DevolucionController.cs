// DevolucionController.cs
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevolucionController : ControllerBase
    {
        private IDevolucionLN _devolucionLN { get; }
        public DevolucionController(IDevolucionLN devolucionLN) { _devolucionLN = devolucionLN; }

        [HttpGet("ListarPorPedido/{pedidoId}")]
        public async Task<IActionResult> ListarPorPedido(int pedidoId)
        {
            var resultado = await _devolucionLN.ListarPorPedidoAsync(pedidoId);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TDevolucion datos)
        {
            var resultado = await _devolucionLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TDevolucion datos)
        {
            var resultado = await _devolucionLN.ModificarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}