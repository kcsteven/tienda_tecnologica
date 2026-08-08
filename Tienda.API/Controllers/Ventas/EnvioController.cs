
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Envio
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class EnvioController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Envio, inyectada por constructor
        private IEnvioLN _envioLN { get; }
        // Constructor: recibe la implementación de IEnvioLN mediante inyección de dependencias
        public EnvioController(IEnvioLN envioLN) { _envioLN = envioLN; }

        // GET: api/Envio/ObtenerPorPedido/{pedidoId}
        // Obtiene el envío asociado a un pedido específico
        [HttpGet("ObtenerPorPedido/{pedidoId}")]
        public async Task<IActionResult> ObtenerPorPedido(int pedidoId)
        {
            // Consulta a la capa de negocio el envío correspondiente al id de pedido recibido
            var resultado = await _envioLN.ObtenerPorPedidoAsync(pedidoId);
            // Si hay error (por ejemplo, no se encontró), responde 404 Not Found
            if (!string.IsNullOrEmpty(resultado.Error)) return NotFound(resultado);
            return Ok(resultado);
        }

        // POST: api/Envio/Insertar
        // Crea un nuevo envío a partir de los datos enviados en el cuerpo de la petición
        // Nota: aquí no se valida ModelState.IsValid antes de insertar
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TEnvio datos)
        {
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _envioLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // PUT: api/Envio/Modificar
        // Actualiza un envío existente con los datos enviados en el cuerpo de la petición
        // Nota: tampoco valida ModelState.IsValid antes de modificar
        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TEnvio datos)
        {
            // Envía la entidad a la capa de negocio para actualizarla
            var resultado = await _envioLN.ModificarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}