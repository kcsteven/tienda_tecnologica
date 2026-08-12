using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagenProductoController : ControllerBase
    {
        private IImagenProductoLN _imagenProductoLN { get; }

        public ImagenProductoController(IImagenProductoLN imagenProductoLN)
        {
            _imagenProductoLN = imagenProductoLN;
        }

        [HttpGet("ListarPorProducto/{productoId}")]
        public async Task<IActionResult> ListarPorProducto(int productoId)
        {
            var resultado = await _imagenProductoLN.ListarPorProductoAsync(productoId);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPost("Insertar")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Insertar([FromBody] TImagenProducto datos)
        {
            var resultado = await _imagenProductoLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpDelete("Eliminar/{id}")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _imagenProductoLN.EliminarAsync(new TImagenProducto { ImagenId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}
