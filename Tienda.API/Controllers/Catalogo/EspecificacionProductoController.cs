using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EspecificacionProductoController : ControllerBase
    {
        private IEspecificacionProductoLN _especificacionLN { get; }

        public EspecificacionProductoController(IEspecificacionProductoLN especificacionLN)
        {
            _especificacionLN = especificacionLN;
        }

        [HttpGet("ListarPorProducto/{productoId}")]
        public async Task<IActionResult> ListarPorProducto(int productoId)
        {
            var resultado = await _especificacionLN.ListarPorProductoAsync(productoId);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TEspecificacionProducto datos)
        {
            var resultado = await _especificacionLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _especificacionLN.EliminarAsync(new TEspecificacionProducto { EspecificacionId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}