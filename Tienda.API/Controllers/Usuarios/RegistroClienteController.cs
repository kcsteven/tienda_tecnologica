using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RegistroClienteController : ControllerBase
{
    private readonly IRegistroClienteLN _registroClienteLN;

    public RegistroClienteController(IRegistroClienteLN registroClienteLN)
    {
        _registroClienteLN = registroClienteLN;
    }

    [HttpGet("TiposDocumento")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> TiposDocumento()
    {
        var resultado = await _registroClienteLN.ListarTiposDocumentoAsync();
        if (!string.IsNullOrEmpty(resultado.Error))
        {
            return BadRequest(resultado);
        }

        return Ok(resultado);
    }

    [HttpPost("Registrar")]
    public async Task<IActionResult> Registrar([FromBody] TRegistroCliente registro)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _registroClienteLN.RegistrarAsync(registro);
        if (!string.IsNullOrEmpty(resultado.Error))
        {
            return BadRequest(resultado);
        }

        return Ok(resultado);
    }
}
