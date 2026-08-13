using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Inventario
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    [Authorize(Roles = "Empleado")]
    public class InventarioController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Inventario, inyectada por constructor
        private IInventarioLN _inventarioLN { get; }

        // Constructor: recibe la implementación de IInventarioLN mediante inyección de dependencias
        public InventarioController(IInventarioLN inventarioLN)
        {
            _inventarioLN = inventarioLN;
        }

        // GET: api/Inventario/Listar
        // Devuelve todos los registros de inventario existentes
        // Nota: a diferencia de otros controladores, aquí no se usa [ResponseCache], por lo que la respuesta podría cachearse
        [HttpGet("Listar")]
        public async Task<IActionResult> Listar()
        {
            // Llama a la capa de negocio para obtener el listado completo de inventario
            var resultado = await _inventarioLN.ListarAsync();
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // GET: api/Inventario/ListarPorProducto/{productoId}
        // Devuelve los registros de inventario asociados a un producto específico
        [HttpGet("ListarPorProducto/{productoId}")]
        public async Task<IActionResult> ListarPorProducto(int productoId)
        {
            // Consulta a la capa de negocio el inventario correspondiente al id de producto recibido
            var resultado = await _inventarioLN.ListarPorProductoAsync(productoId);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpGet("DisponibilidadPublica/{productoId}")]
        [AllowAnonymous]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> DisponibilidadPublica(int productoId)
        {
            var resultado = await _inventarioLN.ListarDisponibilidadPublicaAsync(productoId);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpGet("Obtener/{id}")]
        public async Task<IActionResult> Obtener(int id)
        {
            // Se construye un objeto TInventario solo con el Id para buscarlo en la capa de negocio
            var resultado = await _inventarioLN.ObtenerAsync(new TInventario { InventarioId = id });
            // Si hay error (por ejemplo, no se encontró), responde 404 Not Found
            if (!string.IsNullOrEmpty(resultado.Error)) return NotFound(resultado);
            return Ok(resultado);
        }

        // POST: api/Inventario/Insertar
        // Crea un nuevo registro de inventario a partir de los datos enviados en el cuerpo de la petición
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TInventario inventario)
        {
            // Valida el modelo recibido según las anotaciones de datos (DataAnnotations) definidas en TInventario
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _inventarioLN.InsertarAsync(inventario);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // PUT: api/Inventario/Modificar
        // Actualiza un registro de inventario existente con los datos enviados en el cuerpo de la petición
        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TAjustarInventario inventario)
        {
            // Valida el modelo antes de procesar la modificación
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para actualizarla
            var resultado = await _inventarioLN.ModificarAsync(inventario);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/Inventario/Eliminar/{id}
        // Elimina un registro de inventario existente según su Id
        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TInventario solo con el Id para indicar cuál eliminar
            var resultado = await _inventarioLN.EliminarAsync(new TInventario { InventarioId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}
