using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Dominio.InterfazLN;

namespace Ventas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GarantiaController : ControllerBase
    {
        private IGarantiaLN _garantiaLN { get; }

        public GarantiaController(IGarantiaLN garantiaLN)
        {
            _garantiaLN = garantiaLN;
        }

        [HttpGet("Listar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Listar()
        {
            var resultado = await _garantiaLN.ListarAsync();
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpGet("Obtener/{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Obtener(int id)
        {
            var resultado = await _garantiaLN.ObtenerAsync(new TGarantia { GarantiaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return NotFound(resultado);
            return Ok(resultado);
        }

        [HttpGet("Buscar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(string? nombre)
        {
            var resultado = await _garantiaLN.BuscarAsync(new TGarantia { Nombre = nombre });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TGarantia garantia)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var resultado = await _garantiaLN.InsertarAsync(garantia);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TGarantia garantia)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var resultado = await _garantiaLN.ModificarAsync(garantia);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _garantiaLN.EliminarAsync(new TGarantia { GarantiaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}