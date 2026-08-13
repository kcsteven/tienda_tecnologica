using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Marca
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class MarcaController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Marca, inyectada por constructor
        private IMarcaLN _marcaLN { get; }

        // Constructor: recibe la implementación de IMarcaLN mediante inyección de dependencias
        public MarcaController(IMarcaLN marcaLN)
        {
            _marcaLN = marcaLN;
        }

        // GET: api/Marca/Listar
        // Devuelve todas las marcas existentes
        [HttpGet("Listar")]
        // Evita que la respuesta se almacene en caché (ni en cliente ni en servidor)
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Listar()
        {
            // Llama a la capa de negocio para obtener el listado completo de marcas
            var resultado = await _marcaLN.ListarAsync();
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // GET: api/Marca/Obtener/{id}
        // Obtiene una marca puntual según su Id
        [HttpGet("Obtener/{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Obtener(int id)
        {
            // Se construye un objeto TMarca solo con el Id para buscarla en la capa de negocio
            var resultado = await _marcaLN.ObtenerAsync(new TMarca { MarcaId = id });
            // Si hay error (por ejemplo, no se encontró), responde 404 Not Found
            if (!string.IsNullOrEmpty(resultado.Error)) return NotFound(resultado);
            return Ok(resultado);
        }

        // GET: api/Marca/Buscar?nombre=...
        // Busca marcas cuyo nombre coincida (parcial o total) con el parámetro recibido
        [HttpGet("Buscar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(string nombre)
        {
            // Construye el filtro de búsqueda con el nombre recibido por query string
            var resultado = await _marcaLN.BuscarAsync(new TMarca { Nombre = nombre });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // POST: api/Marca/Insertar
        // Crea una nueva marca a partir de los datos enviados en el cuerpo de la petición
        [HttpPost("Insertar")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Insertar([FromBody] TMarca marca)
        {
            // Valida el modelo recibido según las anotaciones de datos (DataAnnotations) definidas en TMarca
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _marcaLN.InsertarAsync(marca);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // PUT: api/Marca/Modificar
        // Actualiza una marca existente con los datos enviados en el cuerpo de la petición
        [HttpPut("Modificar")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Modificar([FromBody] TMarca marca)
        {
            // Valida el modelo antes de procesar la modificación
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para actualizarla
            var resultado = await _marcaLN.ModificarAsync(marca);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/Marca/Eliminar/{id}
        // Elimina una marca existente según su Id
        [HttpDelete("Eliminar/{id}")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TMarca solo con el Id para indicar cuál eliminar
            var resultado = await _marcaLN.EliminarAsync(new TMarca { MarcaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}
