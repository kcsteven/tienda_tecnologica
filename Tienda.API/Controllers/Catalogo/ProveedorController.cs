using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Proveedor
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    [Authorize(Roles = "Empleado")]
    public class ProveedorController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Proveedor, inyectada por constructor
        private IProveedorLN _proveedorLN { get; }

        // Constructor: recibe la implementación de IProveedorLN mediante inyección de dependencias
        public ProveedorController(IProveedorLN proveedorLN)
        {
            _proveedorLN = proveedorLN;
        }

        // GET: api/Proveedor/Listar
        // Devuelve todos los proveedores existentes
        [HttpGet("Listar")]
        // Evita que la respuesta se almacene en caché (ni en cliente ni en servidor)
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Listar()
        {
            // Llama a la capa de negocio para obtener el listado completo de proveedores
            var resultado = await _proveedorLN.ListarAsync();
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // GET: api/Proveedor/Obtener/{id}
        // Obtiene un proveedor puntual según su Id
        [HttpGet("Obtener/{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Obtener(int id)
        {
            // Se construye un objeto TProveedor solo con el Id para buscarlo en la capa de negocio
            var resultado = await _proveedorLN.ObtenerAsync(new TProveedor { ProveedorId = id });
            // Si hay error (por ejemplo, no se encontró), responde 404 Not Found
            if (!string.IsNullOrEmpty(resultado.Error)) return NotFound(resultado);
            return Ok(resultado);
        }

        // GET: api/Proveedor/Buscar?nombre=...
        // Busca proveedores cuyo nombre coincida (parcial o total) con el parámetro recibido
        [HttpGet("Buscar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(string nombre)
        {
            // Construye el filtro de búsqueda con el nombre recibido por query string
            var resultado = await _proveedorLN.BuscarAsync(new TProveedor { Nombre = nombre });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // POST: api/Proveedor/Insertar
        // Crea un nuevo proveedor a partir de los datos enviados en el cuerpo de la petición
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TProveedor proveedor)
        {
            // Valida el modelo recibido según las anotaciones de datos (DataAnnotations) definidas en TProveedor
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _proveedorLN.InsertarAsync(proveedor);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // PUT: api/Proveedor/Modificar
        // Actualiza un proveedor existente con los datos enviados en el cuerpo de la petición
        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TProveedor proveedor)
        {
            // Valida el modelo antes de procesar la modificación
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para actualizarla
            var resultado = await _proveedorLN.ModificarAsync(proveedor);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/Proveedor/Eliminar/{id}
        // Elimina un proveedor existente según su Id
        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TProveedor solo con el Id para indicar cuál eliminar
            var resultado = await _proveedorLN.EliminarAsync(new TProveedor { ProveedorId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}
