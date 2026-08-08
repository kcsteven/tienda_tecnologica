using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/ImagenProducto
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class ImagenProductoController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de ImagenProducto, inyectada por constructor
        private IImagenProductoLN _imagenProductoLN { get; }

        // Constructor: recibe la implementación de IImagenProductoLN mediante inyección de dependencias
        public ImagenProductoController(IImagenProductoLN imagenProductoLN)
        {
            _imagenProductoLN = imagenProductoLN;
        }

        // GET: api/ImagenProducto/ListarPorProducto/{productoId}
        // Devuelve las imágenes asociadas a un producto específico
        // Nota: este endpoint no tiene [ResponseCache], por lo que la respuesta podría cachearse
        [HttpGet("ListarPorProducto/{productoId}")]
        public async Task<IActionResult> ListarPorProducto(int productoId)
        {
            // Consulta a la capa de negocio las imágenes correspondientes al id de producto recibido
            var resultado = await _imagenProductoLN.ListarPorProductoAsync(productoId);
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // POST: api/ImagenProducto/Insertar
        // Crea una nueva imagen de producto a partir de los datos enviados en el cuerpo de la petición
        // Nota: aquí no se valida ModelState.IsValid antes de insertar
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TImagenProducto datos)
        {
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _imagenProductoLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/ImagenProducto/Eliminar/{id}
        // Elimina una imagen de producto existente según su Id
        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TImagenProducto solo con el Id para indicar cuál eliminar
            var resultado = await _imagenProductoLN.EliminarAsync(new TImagenProducto { ImagenId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}