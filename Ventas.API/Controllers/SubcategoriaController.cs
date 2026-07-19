using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Dominio.InterfazLN;

namespace Ventas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubcategoriaController : ControllerBase
    {
        private ISubcategoriaLN _subcategoriaLN { get; }

        public SubcategoriaController(ISubcategoriaLN subcategoriaLN)
        {
            _subcategoriaLN = subcategoriaLN;
        }

        [HttpGet("Listar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Listar()
        {
            var resultado = await _subcategoriaLN.ListarAsync();
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpGet("ListarPorCategoria/{categoriaId}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> ListarPorCategoria(int categoriaId)
        {
            var resultado = await _subcategoriaLN.ListarPorCategoriaAsync(categoriaId);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpGet("Obtener/{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Obtener(int id)
        {
            var resultado = await _subcategoriaLN.ObtenerAsync(new TSubcategoria { SubcategoriaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return NotFound(resultado);
            return Ok(resultado);
        }

        [HttpGet("Buscar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(string nombre)
        {
            var resultado = await _subcategoriaLN.BuscarAsync(new TSubcategoria { Nombre = nombre });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TSubcategoria subcategoria)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var resultado = await _subcategoriaLN.InsertarAsync(subcategoria);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TSubcategoria subcategoria)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var resultado = await _subcategoriaLN.ModificarAsync(subcategoria);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _subcategoriaLN.EliminarAsync(new TSubcategoria { SubcategoriaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}