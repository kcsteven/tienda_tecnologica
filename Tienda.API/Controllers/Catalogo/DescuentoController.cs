using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Descuento
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class DescuentoController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Descuento, inyectada por constructor
        private IDescuentoLN _descuentoLN { get; }

        // Constructor: recibe la implementación de IDescuentoLN mediante inyección de dependencias
        public DescuentoController(IDescuentoLN descuentoLN)
        {
            _descuentoLN = descuentoLN;
        }

        // GET: api/Descuento/Listar
        // Devuelve todos los descuentos existentes
        [HttpGet("Listar")]
        // Evita que la respuesta se almacene en caché (ni en cliente ni en servidor)
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Listar()
        {
            // Llama a la capa de negocio para obtener el listado completo de descuentos
            var resultado = await _descuentoLN.ListarAsync();
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // GET: api/Descuento/Obtener/{id}
        // Obtiene un descuento puntual según su Id
        [HttpGet("Obtener/{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Obtener(int id)
        {
            // Se construye un objeto TDescuento solo con el Id para buscarlo en la capa de negocio
            var resultado = await _descuentoLN.ObtenerAsync(new TDescuento { DescuentoId = id });
            // Si hay error (por ejemplo, no se encontró), responde 404 Not Found
            if (!string.IsNullOrEmpty(resultado.Error)) return NotFound(resultado);
            return Ok(resultado);
        }

        // GET: api/Descuento/Buscar?nombre=...
        // Busca descuentos cuyo nombre coincida (parcial o total) con el parámetro recibido
        [HttpGet("Buscar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(string nombre)
        {
            // Construye el filtro de búsqueda con el nombre recibido por query string
            var resultado = await _descuentoLN.BuscarAsync(new TDescuento { Nombre = nombre });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // POST: api/Descuento/Insertar
        // Crea un nuevo descuento a partir de los datos enviados en el cuerpo de la petición
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TDescuento descuento)
        {
            // Valida el modelo recibido según las anotaciones de datos (DataAnnotations) definidas en TDescuento
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _descuentoLN.InsertarAsync(descuento);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // PUT: api/Descuento/Modificar
        // Actualiza un descuento existente con los datos enviados en el cuerpo de la petición
        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TDescuento descuento)
        {
            // Valida el modelo antes de procesar la modificación
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para actualizarla
            var resultado = await _descuentoLN.ModificarAsync(descuento);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/Descuento/Eliminar/{id}
        // Elimina un descuento existente según su Id
        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TDescuento solo con el Id para indicar cuál eliminar
            var resultado = await _descuentoLN.EliminarAsync(new TDescuento { DescuentoId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}