using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Dominio.InterfazLN;

namespace Ventas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private ICategoriaLN _categoriaLN { get; }


        public CategoriaController(ICategoriaLN categoriaLN)

        {

            _categoriaLN = categoriaLN;

        }



        [HttpGet("Listar")]

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

        public async Task<IActionResult> Listar()

        {

            var resultado = await _categoriaLN.ListarAsync();


            if (!string.IsNullOrEmpty(resultado.Error))

                return BadRequest(resultado);


            return Ok(resultado);

        }


        [HttpGet("Obtener/{id}")]

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

        public async Task<IActionResult> Obtener(int id)

        {

            var resultado = await _categoriaLN.ObtenerAsync(

                new TCategorium
                {

                    CategoriaId = id
                });


            if (!string.IsNullOrEmpty(resultado.Error))

                return NotFound(resultado);


            return Ok(resultado);

        }


        [HttpGet("Buscar")]

        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

        public async Task<IActionResult> Buscar(string nombreCategoria)

        {

            var resultado = await _categoriaLN.BuscarAsync(

                new TCategorium
                {

                    NombreCategoria = nombreCategoria
                });


            if (!string.IsNullOrEmpty(resultado.Error))

                return BadRequest(resultado);


            return Ok(resultado);

        }


        [HttpPost("Insertar")]

        public async Task<IActionResult> Insertar([FromBody] TCategorium categoria)

        {

            if (!ModelState.IsValid)

                return BadRequest(ModelState);


            var resultado = await _categoriaLN.InsertarAsync(categoria);


            if (!string.IsNullOrEmpty(resultado.Error))

                return BadRequest(resultado);


            return Ok(resultado);

        }


        [HttpPut("Modificar")]

        public async Task<IActionResult> Modificar([FromBody] TCategorium categoria)

        {

            if (!ModelState.IsValid)

                return BadRequest(ModelState);


            var resultado = await _categoriaLN.ModificarAsync(categoria);


            if (!string.IsNullOrEmpty(resultado.Error))

                return BadRequest(resultado);


            return Ok(resultado);

        }


        [HttpDelete("Eliminar/{id}")]

        public async Task<IActionResult> Eliminar(int id)

        {

            var resultado = await _categoriaLN.EliminarAsync(

                new TCategorium
                {

                    CategoriaId = id
                });


            if (!string.IsNullOrEmpty(resultado.Error))

                return BadRequest(resultado);


            return Ok(resultado);

        }

    }
}

