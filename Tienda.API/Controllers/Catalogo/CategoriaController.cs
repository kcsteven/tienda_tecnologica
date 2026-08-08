using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Categoria
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Categoria, inyectada por constructor
        private ICategoriaLN _categoriaLN { get; }

        // Constructor: recibe la implementación de ICategoriaLN mediante inyección de dependencias
        public CategoriaController(ICategoriaLN categoriaLN)
        {
            _categoriaLN = categoriaLN;
        }

        // GET: api/Categoria/Listar
        // Devuelve todas las categorías existentes
        [HttpGet("Listar")]
        // Evita que la respuesta se almacene en caché (ni en cliente ni en servidor)
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Listar()
        {
            // Llama a la capa de negocio para obtener el listado completo de categorías
            var resultado = await _categoriaLN.ListarAsync();
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // GET: api/Categoria/Obtener/{id}
        // Obtiene una categoría puntual según su Id
        [HttpGet("Obtener/{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Obtener(int id)
        {
            // Se construye un objeto TCategoria solo con el Id para buscarla en la capa de negocio
            var resultado = await _categoriaLN.ObtenerAsync(new TCategoria { CategoriaId = id });
            // Si hay error (por ejemplo, no se encontró), responde 404 Not Found
            if (!string.IsNullOrEmpty(resultado.Error)) return NotFound(resultado);
            return Ok(resultado);
        }

        // GET: api/Categoria/Buscar?nombre=...
        // Busca categorías cuyo nombre coincida (parcial o total) con el parámetro recibido
        [HttpGet("Buscar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(string nombre)
        {
            // Construye el filtro de búsqueda con el nombre recibido por query string
            var resultado = await _categoriaLN.BuscarAsync(new TCategoria { Nombre = nombre });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // POST: api/Categoria/Insertar
        // Crea una nueva categoría a partir de los datos enviados en el cuerpo de la petición
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TCategoria categoria)
        {
            // Valida el modelo recibido según las anotaciones de datos (DataAnnotations) definidas en TCategoria
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _categoriaLN.InsertarAsync(categoria);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // PUT: api/Categoria/Modificar
        // Actualiza una categoría existente con los datos enviados en el cuerpo de la petición
        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TCategoria categoria)
        {
            // Valida el modelo antes de procesar la modificación
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para actualizarla
            var resultado = await _categoriaLN.ModificarAsync(categoria);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/Categoria/Eliminar/{id}
        // Elimina una categoría existente según su Id
        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TCategoria solo con el Id para indicar cuál eliminar
            var resultado = await _categoriaLN.EliminarAsync(new TCategoria { CategoriaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}