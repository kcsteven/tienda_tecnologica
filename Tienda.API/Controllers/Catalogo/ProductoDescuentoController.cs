using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/ProductoDescuento
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class ProductoDescuentoController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de ProductoDescuento, inyectada por constructor
        private IProductoDescuentoLN _productoDescuentoLN { get; }

        // Constructor: recibe la implementación de IProductoDescuentoLN mediante inyección de dependencias
        public ProductoDescuentoController(IProductoDescuentoLN productoDescuentoLN)
        {
            _productoDescuentoLN = productoDescuentoLN;
        }

        // GET: api/ProductoDescuento/ListarPorProducto/{productoId}
        // Devuelve los descuentos asociados a un producto específico
        // Nota: este endpoint no tiene [ResponseCache], por lo que la respuesta podría cachearse
        [HttpGet("ListarPorProducto/{productoId}")]
        public async Task<IActionResult> ListarPorProducto(int productoId)
        {
            // Consulta a la capa de negocio los descuentos correspondientes al id de producto recibido
            var resultado = await _productoDescuentoLN.ListarPorProductoAsync(productoId);
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // POST: api/ProductoDescuento/Insertar
        // Crea una nueva asociación producto-descuento a partir de los datos enviados en el cuerpo de la petición
        // Nota: aquí no se valida ModelState.IsValid antes de insertar
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TProductoDescuento datos)
        {
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _productoDescuentoLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/ProductoDescuento/Eliminar/{id}
        // Elimina una asociación producto-descuento existente según su Id
        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TProductoDescuento solo con el Id para indicar cuál eliminar
            var resultado = await _productoDescuentoLN.EliminarAsync(new TProductoDescuento { ProductoDescuentoId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}