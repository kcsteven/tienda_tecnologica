using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Pedido
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class PedidoController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Pedido, inyectada por constructor
        private IPedidoLN _pedidoLN { get; }

        // Constructor: recibe la implementación de IPedidoLN mediante inyección de dependencias
        public PedidoController(IPedidoLN pedidoLN)
        {
            _pedidoLN = pedidoLN;
        }

        // GET: api/Pedido/Listar
        // Devuelve todos los pedidos existentes
        [HttpGet("Listar")]
        // Evita que la respuesta se almacene en caché (ni en cliente ni en servidor)
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Listar()
        {
            // Llama a la capa de negocio para obtener el listado completo de pedidos
            var resultado = await _pedidoLN.ListarAsync();
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // GET: api/Pedido/Obtener/{id}
        // Obtiene un pedido puntual según su Id
        [HttpGet("Obtener/{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Obtener(int id)
        {
            // Se construye un objeto TPedido solo con el Id para buscarlo en la capa de negocio
            var resultado = await _pedidoLN.ObtenerAsync(
                new TPedido
                {
                    PedidoId = id
                });
            // Si hay error (por ejemplo, no se encontró), responde 404 Not Found
            if (!string.IsNullOrEmpty(resultado.Error))
                return NotFound(resultado);
            return Ok(resultado);
        }

        // GET: api/Pedido/Buscar?nombrePedido=...
        // Busca pedidos cuyo nombre coincida (parcial o total) con el parámetro recibido
        [HttpGet("Buscar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(string nombrePedido)
        {
            // Construye el filtro de búsqueda con el nombre de pedido recibido por query string
            var resultado = await _pedidoLN.BuscarAsync(
                new TPedido
                {
                    NombrePedido = nombrePedido
                });
            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);
            return Ok(resultado);
        }

        // POST: api/Pedido/Insertar
        // Crea un nuevo pedido a partir de los datos enviados en el cuerpo de la petición
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TPedido pedido)
        {
            // Valida el modelo recibido según las anotaciones de datos (DataAnnotations) definidas en TPedido
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _pedidoLN.InsertarAsync(pedido);
            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // PUT: api/Pedido/Modificar
        // Actualiza un pedido existente con los datos enviados en el cuerpo de la petición
        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TPedido pedido)
        {
            // Valida el modelo antes de procesar la modificación
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Envía la entidad a la capa de negocio para actualizarla
            var resultado = await _pedidoLN.ModificarAsync(pedido);
            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            return Ok(resultado);
        }

        // DELETE: api/Pedido/Eliminar/{id}
        // Elimina un pedido existente según su Id
        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TPedido solo con el Id para indicar cuál eliminar
            var resultado = await _pedidoLN.EliminarAsync(
                new TPedido
                {
                    PedidoId = id
                });

            if (!string.IsNullOrEmpty(resultado.Error))
                return BadRequest(resultado);

            return Ok(resultado);
        }

    }
}