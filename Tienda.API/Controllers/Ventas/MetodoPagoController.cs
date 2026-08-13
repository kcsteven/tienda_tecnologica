using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers.Ventas;

[ApiController]
[Route("api/[controller]")]
public class MetodoPagoController : ControllerBase
{
    private readonly IMetodoPagoLN _metodoPagoLN;

    public MetodoPagoController(IMetodoPagoLN metodoPagoLN)
    {
        _metodoPagoLN = metodoPagoLN;
    }

    [HttpGet("Listar")]
    public async Task<IActionResult> Listar()
    {
        var resultado = await _metodoPagoLN.ListarAsync();

        if (!string.IsNullOrEmpty(resultado.Error))
            return BadRequest(resultado);

        return Ok(resultado);
    }
}