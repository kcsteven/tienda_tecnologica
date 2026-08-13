using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Garantia
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class GarantiaController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Garantia, inyectada por constructor
        private IGarantiaLN _garantiaLN { get; }

        // Constructor: recibe la implementación de IGarantiaLN mediante inyección de dependencias
        public GarantiaController(IGarantiaLN garantiaLN)
        {
            _garantiaLN = garantiaLN;
        }

        // GET: api/Garantia/Listar
        // Devuelve todas las garantías existentes
        [HttpGet("Listar")]
        // Evita que la respuesta se almacene en caché (ni en cliente ni en servidor)
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Listar()
        {
            // Llama a la capa de negocio para obtener el listado completo de garantías
            var resultado = await _garantiaLN.ListarAsync();
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // GET: api/Garantia/Obtener/{id}
        // Obtiene una garantía puntual según su Id
        [HttpGet("Obtener/{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Obtener(int id)
        {
            // Se construye un objeto TGarantia solo con el Id para buscarla en la capa de negocio
            var resultado = await _garantiaLN.ObtenerAsync(new TGarantia { GarantiaId = id });
            // Si hay error (por ejemplo, no se encontró), responde 404 Not Found
            if (!string.IsNullOrEmpty(resultado.Error)) return NotFound(resultado);
            return Ok(resultado);
        }

        // GET: api/Garantia/Buscar?nombre=...
        // Busca garantías cuyo nombre coincida (parcial o total) con el parámetro recibido
        // Nota: el parámetro "nombre" es nullable (string?), por lo que puede omitirse en la query string
        [HttpGet("Buscar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(string? nombre)
        {
            // Construye el filtro de búsqueda con el nombre recibido por query string
            var resultado = await _garantiaLN.BuscarAsync(new TGarantia { Nombre = nombre });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // POST: api/Garantia/Insertar
        // Crea una nueva garantía a partir de los datos enviados en el cuerpo de la petición
        [HttpPost("Insertar")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Insertar([FromBody] TGarantia garantia)
        {
            // Valida el modelo recibido según las anotaciones de datos (DataAnnotations) definidas en TGarantia
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _garantiaLN.InsertarAsync(garantia);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // PUT: api/Garantia/Modificar
        // Actualiza una garantía existente con los datos enviados en el cuerpo de la petición
        [HttpPut("Modificar")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Modificar([FromBody] TGarantia garantia)
        {
            // Valida el modelo antes de procesar la modificación
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para actualizarla
            var resultado = await _garantiaLN.ModificarAsync(garantia);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/Garantia/Eliminar/{id}
        // Elimina una garantía existente según su Id
        [HttpDelete("Eliminar/{id}")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TGarantia solo con el Id para indicar cuál eliminar
            var resultado = await _garantiaLN.EliminarAsync(new TGarantia { GarantiaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}
