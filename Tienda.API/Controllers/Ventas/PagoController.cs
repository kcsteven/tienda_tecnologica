using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers.Ventas;

[ApiController]
[Route("api/[controller]")]
public class PagoController : ControllerBase
{
    private readonly IPagoLN _pagoLN;

    public PagoController(IPagoLN pagoLN)
    {
        _pagoLN = pagoLN;
    }

    [HttpGet("ListarPorPedido/{pedidoId}")]
    public async Task<IActionResult> ListarPorPedido(int pedidoId)
    {
        var resultado = await _pagoLN.ListarPorPedidoAsync(pedidoId);

        if (!string.IsNullOrEmpty(resultado.Error))
            return BadRequest(resultado);

        return Ok(resultado);
    }

    [HttpPost("Insertar")]
    public async Task<IActionResult> Insertar(
        [FromBody] TPago datos)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var resultado = await _pagoLN.InsertarAsync(datos);

        if (!string.IsNullOrEmpty(resultado.Error))
            return BadRequest(resultado);

        return Ok(resultado);
    }

    [HttpDelete("Eliminar/{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await _pagoLN.EliminarAsync(
            new TPago
            {
                PagoId = id
            });

        if (!string.IsNullOrEmpty(resultado.Error))
            return BadRequest(resultado);

        return Ok(resultado);
    }
}