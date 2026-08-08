using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Subcategoria
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class SubcategoriaController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Subcategoria, inyectada por constructor
        private ISubcategoriaLN _subcategoriaLN { get; }

        // Constructor: recibe la implementación de ISubcategoriaLN mediante inyección de dependencias
        public SubcategoriaController(ISubcategoriaLN subcategoriaLN)
        {
            _subcategoriaLN = subcategoriaLN;
        }

        // GET: api/Subcategoria/Listar
        // Devuelve todas las subcategorías existentes
        [HttpGet("Listar")]
        // Evita que la respuesta se almacene en caché (ni en cliente ni en servidor)
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Listar()
        {
            // Llama a la capa de negocio para obtener el listado completo
            var resultado = await _subcategoriaLN.ListarAsync();
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // GET: api/Subcategoria/ListarPorCategoria/{categoriaId}
        // Devuelve las subcategorías filtradas por una categoría específica
        [HttpGet("ListarPorCategoria/{categoriaId}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> ListarPorCategoria(int categoriaId)
        {
            // Consulta a la capa de negocio las subcategorías asociadas al id de categoría recibido
            var resultado = await _subcategoriaLN.ListarPorCategoriaAsync(categoriaId);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // GET: api/Subcategoria/Obtener/{id}
        // Obtiene una subcategoría puntual según su Id
        [HttpGet("Obtener/{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Obtener(int id)
        {
            // Se construye un objeto TSubcategoria solo con el Id para buscarla en la capa de negocio
            var resultado = await _subcategoriaLN.ObtenerAsync(new TSubcategoria { SubcategoriaId = id });
            // Si hay error (por ejemplo, no se encontró), responde 404 Not Found
            if (!string.IsNullOrEmpty(resultado.Error)) return NotFound(resultado);
            return Ok(resultado);
        }

        // GET: api/Subcategoria/Buscar?nombre=...
        // Busca subcategorías cuyo nombre coincida (parcial o total) con el parámetro recibido
        [HttpGet("Buscar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(string nombre)
        {
            // Construye el filtro de búsqueda con el nombre recibido por query string
            var resultado = await _subcategoriaLN.BuscarAsync(new TSubcategoria { Nombre = nombre });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // POST: api/Subcategoria/Insertar
        // Crea una nueva subcategoría a partir de los datos enviados en el cuerpo de la petición
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TSubcategoria subcategoria)
        {
            // Valida el modelo recibido según las anotaciones de datos (DataAnnotations) definidas en TSubcategoria
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _subcategoriaLN.InsertarAsync(subcategoria);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // PUT: api/Subcategoria/Modificar
        // Actualiza una subcategoría existente con los datos enviados en el cuerpo de la petición
        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TSubcategoria subcategoria)
        {
            // Valida el modelo antes de procesar la modificación
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para actualizarla
            var resultado = await _subcategoriaLN.ModificarAsync(subcategoria);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/Subcategoria/Eliminar/{id}
        // Elimina una subcategoría existente según su Id
        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TSubcategoria solo con el Id para indicar cuál eliminar
            var resultado = await _subcategoriaLN.EliminarAsync(new TSubcategoria { SubcategoriaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}