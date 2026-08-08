
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Resena
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class ResenaController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Resena, inyectada por constructor
        private IResenaLN _resenaLN { get; }
        // Constructor: recibe la implementación de IResenaLN mediante inyección de dependencias
        public ResenaController(IResenaLN resenaLN) { _resenaLN = resenaLN; }

        // GET: api/Resena/ListarPorProducto/{productoId}
        // Devuelve las reseñas asociadas a un producto específico
        // Nota: este endpoint no tiene [ResponseCache], por lo que la respuesta podría cachearse
        [HttpGet("ListarPorProducto/{productoId}")]
        public async Task<IActionResult> ListarPorProducto(int productoId)
        {
            // Consulta a la capa de negocio las reseñas correspondientes al id de producto recibido
            var resultado = await _resenaLN.ListarPorProductoAsync(productoId);
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // POST: api/Resena/Insertar
        // Crea una nueva reseña de producto a partir de los datos enviados en el cuerpo de la petición
        // Nota: aquí no se valida ModelState.IsValid antes de insertar
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TResena datos)
        {
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _resenaLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/Resena/Eliminar/{id}
        // Elimina una reseña existente según su Id
        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TResena solo con el Id para indicar cuál eliminar
            var resultado = await _resenaLN.EliminarAsync(new TResena { ResenaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}