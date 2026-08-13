using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/ProductoEtiqueta
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class ProductoEtiquetaController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de ProductoEtiqueta, inyectada por constructor
        private IProductoEtiquetaLN _productoEtiquetaLN { get; }

        // Constructor: recibe la implementación de IProductoEtiquetaLN mediante inyección de dependencias
        public ProductoEtiquetaController(IProductoEtiquetaLN productoEtiquetaLN)
        {
            _productoEtiquetaLN = productoEtiquetaLN;
        }

        // GET: api/ProductoEtiqueta/ListarPorProducto/{productoId}
        // Devuelve las etiquetas asociadas a un producto específico
        // Nota: este endpoint no tiene [ResponseCache], por lo que la respuesta podría cachearse
        [HttpGet("ListarPorProducto/{productoId}")]
        public async Task<IActionResult> ListarPorProducto(int productoId)
        {
            // Consulta a la capa de negocio las etiquetas correspondientes al id de producto recibido
            var resultado = await _productoEtiquetaLN.ListarPorProductoAsync(productoId);
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // POST: api/ProductoEtiqueta/Insertar
        // Crea una nueva asociación producto-etiqueta a partir de los datos enviados en el cuerpo de la petición
        // Nota: aquí no se valida ModelState.IsValid antes de insertar
        [HttpPost("Insertar")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Insertar([FromBody] TProductoEtiqueta datos)
        {
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _productoEtiquetaLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/ProductoEtiqueta/Eliminar/{id}
        // Elimina una asociación producto-etiqueta existente según su Id
        [HttpDelete("Eliminar/{id}")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TProductoEtiqueta solo con el Id para indicar cuál eliminar
            var resultado = await _productoEtiquetaLN.EliminarAsync(new TProductoEtiqueta { ProductoEtiquetaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}
