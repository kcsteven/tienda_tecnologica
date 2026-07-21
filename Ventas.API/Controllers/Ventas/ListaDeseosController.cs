// ListaDeseosController.cs
using Microsoft.AspNetCore.Mvc;
using Ventas.Dominio.EntidadesTipadas;
using Ventas.Dominio.InterfazLN;

namespace Ventas.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListaDeseosController : ControllerBase
    {
        private IListaDeseosLN _listaDeseosLN { get; }
        public ListaDeseosController(IListaDeseosLN listaDeseosLN) { _listaDeseosLN = listaDeseosLN; }

        [HttpGet("ListarPorCliente/{clienteId}")]
        public async Task<IActionResult> ListarPorCliente(int clienteId)
        {
            var resultado = await _listaDeseosLN.ListarPorClienteAsync(clienteId);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TListaDeseos datos)
        {
            var resultado = await _listaDeseosLN.InsertarAsync(datos);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _listaDeseosLN.EliminarAsync(new TListaDeseos { ListaDeseosId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}