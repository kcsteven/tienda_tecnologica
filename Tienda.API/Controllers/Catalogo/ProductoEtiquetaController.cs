using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoEtiquetaController : ControllerBase
    {
        private IProductoEtiquetaLN _productoEtiquetaLN { get; }

        public ProductoEtiquetaController(IProductoEtiquetaLN productoEtiquetaLN)
        {
            _productoEtiquetaLN = productoEtiquetaLN;
        }

        [HttpGet("ListarPorProducto/{productoId}")]
        public async Task<IActionResult> ListarPorProducto(int productoId)
        {
            var resultado = await _productoEtiquetaLN.ListarPorProductoAsync(productoId);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPost("Insertar")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Insertar([FromBody] TProductoEtiqueta datos)
        {
            var resultado = await _productoEtiquetaLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpDelete("Eliminar/{id}")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _productoEtiquetaLN.EliminarAsync(new TProductoEtiqueta { ProductoEtiquetaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}
