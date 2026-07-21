using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Dominio.InterfazLN;

namespace Ventas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventarioController : ControllerBase
    {
        private IInventarioLN _inventarioLN { get; }

        public InventarioController(IInventarioLN inventarioLN)
        {
            _inventarioLN = inventarioLN;
        }

        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            var resultado = await _inventarioLN.ListarAsync();
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpGet("ListarPorProducto/{productoId}")]
        public async Task<IActionResult> ListarPorProducto(int productoId)
        {
            var resultado = await _inventarioLN.ListarPorProductoAsync(productoId);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpGet("Obtener/{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var resultado = await _inventarioLN.ObtenerAsync(new TInventario { InventarioId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return NotFound(resultado);
            return Ok(resultado);
        }

        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TInventario inventario)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var resultado = await _inventarioLN.InsertarAsync(inventario);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TInventario inventario)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var resultado = await _inventarioLN.ModificarAsync(inventario);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _inventarioLN.EliminarAsync(new TInventario { InventarioId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}