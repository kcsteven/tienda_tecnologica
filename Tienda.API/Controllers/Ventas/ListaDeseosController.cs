
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/ListaDeseos
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class ListaDeseosController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de ListaDeseos, inyectada por constructor
        private IListaDeseosLN _listaDeseosLN { get; }
        // Constructor: recibe la implementación de IListaDeseosLN mediante inyección de dependencias
        public ListaDeseosController(IListaDeseosLN listaDeseosLN) { _listaDeseosLN = listaDeseosLN; }

        // GET: api/ListaDeseos/ListarPorCliente/{clienteId}
        // Devuelve los productos en la lista de deseos asociados a un cliente específico
        // Nota: este endpoint no tiene [ResponseCache], por lo que la respuesta podría cachearse
        [HttpGet("ListarPorCliente/{clienteId}")]
        public async Task<IActionResult> ListarPorCliente(int clienteId)
        {
            // Consulta a la capa de negocio la lista de deseos correspondiente al id de cliente recibido
            var resultado = await _listaDeseosLN.ListarPorClienteAsync(clienteId);
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // POST: api/ListaDeseos/Insertar
        // Agrega un nuevo elemento a la lista de deseos a partir de los datos enviados en el cuerpo de la petición
        // Nota: aquí no se valida ModelState.IsValid antes de insertar
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TListaDeseos datos)
        {
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _listaDeseosLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/ListaDeseos/Eliminar/{id}
        // Elimina un elemento de la lista de deseos existente según su Id
        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TListaDeseos solo con el Id para indicar cuál eliminar
            var resultado = await _listaDeseosLN.EliminarAsync(new TListaDeseos { ListaDeseosId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}