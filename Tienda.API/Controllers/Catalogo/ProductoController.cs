using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Producto
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class ProductoController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Producto, inyectada por constructor
        private IProductoLN _productoLN { get; }

        // Constructor: recibe la implementación de IProductoLN mediante inyección de dependencias
        public ProductoController(IProductoLN productoLN)
        {
            _productoLN = productoLN;
        }

        // GET: api/Producto/Listar
        // Devuelve todos los productos existentes
        [HttpGet("Listar")]
        // Evita que la respuesta se almacene en caché (ni en cliente ni en servidor)
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Listar()
        {
            // Llama a la capa de negocio para obtener el listado completo de productos
            var resultado = await _productoLN.ListarAsync();
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // GET: api/Producto/Obtener/{id}
        // Obtiene un producto puntual según su Id
        [HttpGet("Obtener/{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Obtener(int id)
        {
            // Se construye un objeto TProducto solo con el Id para buscarlo en la capa de negocio
            var resultado = await _productoLN.ObtenerAsync(
                new TProducto
                {
                    ProductoId = id
                });

            // Si hay error (por ejemplo, no se encontró), responde 404 Not Found
            if (!string.IsNullOrEmpty(resultado.Error))
                return NotFound(resultado);

            return Ok(resultado);
        }

        [HttpGet("ListarAdministracion")]
        [Authorize(Roles = "Empleado")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> ListarAdministracion()
        {
            var resultado = await _productoLN.ListarAdministracionAsync();
            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpGet("Buscar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(string nombreProducto)
        {
            // Construye el filtro de búsqueda con el nombre recibido por query string
            var resultado = await _productoLN.BuscarAsync(
                new TProducto
                {
                    Nombre = nombreProducto
                });

            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // POST: api/Producto/Insertar
        // Crea un nuevo producto a partir de los datos enviados en el cuerpo de la petición
        [HttpPost("Insertar")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Insertar([FromBody] TCrearProductoConInventario producto)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _productoLN.InsertarAsync(producto, usuarioId);

            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // PUT: api/Producto/Modificar
        // Actualiza un producto existente con los datos enviados en el cuerpo de la petición
        [HttpPut("Modificar")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Modificar([FromBody] TActualizarProducto producto)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _productoLN.ModificarAsync(producto, usuarioId);

            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpPut("CambiarEstado")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> CambiarEstado([FromBody] TCambiarEstadoProducto producto)
        {
            if (!TryObtenerUsuarioId(out var usuarioId))
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resultado = await _productoLN.CambiarEstadoAsync(producto, usuarioId);
            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpDelete("Eliminar/{id}")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Eliminar(int id)
        {
            return Conflict(new Respuesta<bool>
            {
                Error = "No se permite eliminar productos físicamente. Utilice la desactivación."
            });
        }

        private bool TryObtenerUsuarioId(out int usuarioId)
        {
            usuarioId = 0;
            var valor = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(
                valor,
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out usuarioId)
                && usuarioId > 0;
        }
    }
}