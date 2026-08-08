
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Pago
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class PagoController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Pago, inyectada por constructor
        private IPagoLN _pagoLN { get; }
        // Constructor: recibe la implementación de IPagoLN mediante inyección de dependencias
        public PagoController(IPagoLN pagoLN) { _pagoLN = pagoLN; }

        // GET: api/Pago/ListarPorPedido/{pedidoId}
        // Devuelve los pagos asociados a un pedido específico
        // Nota: este endpoint no tiene [ResponseCache], por lo que la respuesta podría cachearse
        [HttpGet("ListarPorPedido/{pedidoId}")]
        public async Task<IActionResult> ListarPorPedido(int pedidoId)
        {
            // Consulta a la capa de negocio los pagos correspondientes al id de pedido recibido
            var resultado = await _pagoLN.ListarPorPedidoAsync(pedidoId);
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // POST: api/Pago/Insertar
        // Crea un nuevo pago a partir de los datos enviados en el cuerpo de la petición
        // Nota: aquí no se valida ModelState.IsValid antes de insertar
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TPago datos)
        {
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _pagoLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/Pago/Eliminar/{id}
        // Elimina un pago existente según su Id
        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TPago solo con el Id para indicar cuál eliminar
            var resultado = await _pagoLN.EliminarAsync(new TPago { PagoId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}