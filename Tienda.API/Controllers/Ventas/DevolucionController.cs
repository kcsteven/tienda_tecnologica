
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Devolucion
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class DevolucionController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Devolucion, inyectada por constructor
        private IDevolucionLN _devolucionLN { get; }
        // Constructor: recibe la implementación de IDevolucionLN mediante inyección de dependencias
        public DevolucionController(IDevolucionLN devolucionLN) { _devolucionLN = devolucionLN; }

        // GET: api/Devolucion/ListarPorPedido/{pedidoId}
        // Devuelve las devoluciones asociadas a un pedido específico
        // Nota: este endpoint no tiene [ResponseCache], por lo que la respuesta podría cachearse
        [HttpGet("ListarPorPedido/{pedidoId}")]
        public async Task<IActionResult> ListarPorPedido(int pedidoId)
        {
            // Consulta a la capa de negocio las devoluciones correspondientes al id de pedido recibido
            var resultado = await _devolucionLN.ListarPorPedidoAsync(pedidoId);
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // POST: api/Devolucion/Insertar
        // Crea una nueva devolución a partir de los datos enviados en el cuerpo de la petición
        // Nota: aquí no se valida ModelState.IsValid antes de insertar
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TDevolucion datos)
        {
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _devolucionLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // PUT: api/Devolucion/Modificar
        // Actualiza una devolución existente con los datos enviados en el cuerpo de la petición
        // Nota: tampoco valida ModelState.IsValid antes de modificar
        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TDevolucion datos)
        {
            // Envía la entidad a la capa de negocio para actualizarla
            var resultado = await _devolucionLN.ModificarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}