using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoGarantiaController : ControllerBase
    {
        private IProductoGarantiaLN _productoGarantiaLN { get; }

        public ProductoGarantiaController(IProductoGarantiaLN productoGarantiaLN)
        {
            _productoGarantiaLN = productoGarantiaLN;
        }

        [HttpGet("ListarPorProducto/{productoId}")]
        public async Task<IActionResult> ListarPorProducto(int productoId)
        {
            var resultado = await _productoGarantiaLN.ListarPorProductoAsync(productoId);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TProductoGarantia datos)
        {
            var resultado = await _productoGarantiaLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _productoGarantiaLN.EliminarAsync(new TProductoGarantia { ProductoGarantiaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}