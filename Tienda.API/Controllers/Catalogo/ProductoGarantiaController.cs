using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/ProductoGarantia
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class ProductoGarantiaController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de ProductoGarantia, inyectada por constructor
        private IProductoGarantiaLN _productoGarantiaLN { get; }

        // Constructor: recibe la implementación de IProductoGarantiaLN mediante inyección de dependencias
        public ProductoGarantiaController(IProductoGarantiaLN productoGarantiaLN)
        {
            _productoGarantiaLN = productoGarantiaLN;
        }

        // GET: api/ProductoGarantia/ListarPorProducto/{productoId}
        // Devuelve las garantías asociadas a un producto específico
        // Nota: a diferencia de otros controladores, este endpoint no tiene [ResponseCache], por lo que sí podría cachearse la respuesta
        [HttpGet("ListarPorProducto/{productoId}")]
        public async Task<IActionResult> ListarPorProducto(int productoId)
        {
            // Consulta a la capa de negocio las garantías correspondientes al id de producto recibido
            var resultado = await _productoGarantiaLN.ListarPorProductoAsync(productoId);
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // POST: api/ProductoGarantia/Insertar
        // Crea una nueva garantía de producto a partir de los datos enviados en el cuerpo de la petición
        // Nota: a diferencia de otros controladores, aquí no se valida ModelState.IsValid antes de insertar
        [HttpPost("Insertar")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Insertar([FromBody] TProductoGarantia datos)
        {
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _productoGarantiaLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/ProductoGarantia/Eliminar/{id}
        // Elimina una garantía de producto existente según su Id
        [HttpDelete("Eliminar/{id}")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TProductoGarantia solo con el Id para indicar cuál eliminar
            var resultado = await _productoGarantiaLN.EliminarAsync(new TProductoGarantia { ProductoGarantiaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}
