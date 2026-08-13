using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/EspecificacionProducto
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class EspecificacionProductoController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de EspecificacionProducto, inyectada por constructor
        private IEspecificacionProductoLN _especificacionLN { get; }

        // Constructor: recibe la implementación de IEspecificacionProductoLN mediante inyección de dependencias
        public EspecificacionProductoController(IEspecificacionProductoLN especificacionLN)
        {
            _especificacionLN = especificacionLN;
        }

        // GET: api/EspecificacionProducto/ListarPorProducto/{productoId}
        // Devuelve las especificaciones técnicas asociadas a un producto específico
        // Nota: este endpoint no tiene [ResponseCache], por lo que la respuesta podría cachearse
        [HttpGet("ListarPorProducto/{productoId}")]
        public async Task<IActionResult> ListarPorProducto(int productoId)
        {
            // Consulta a la capa de negocio las especificaciones correspondientes al id de producto recibido
            var resultado = await _especificacionLN.ListarPorProductoAsync(productoId);
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // POST: api/EspecificacionProducto/Insertar
        // Crea una nueva especificación de producto a partir de los datos enviados en el cuerpo de la petición
        // Nota: aquí no se valida ModelState.IsValid antes de insertar
        [HttpPost("Insertar")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Insertar([FromBody] TEspecificacionProducto datos)
        {
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _especificacionLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/EspecificacionProducto/Eliminar/{id}
        // Elimina una especificación de producto existente según su Id
        [HttpDelete("Eliminar/{id}")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TEspecificacionProducto solo con el Id para indicar cuál eliminar
            var resultado = await _especificacionLN.EliminarAsync(new TEspecificacionProducto { EspecificacionId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}
