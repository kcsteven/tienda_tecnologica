using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers.Usuarios;

[ApiController]
[Route("api/[controller]")]
public class AccesoUsuarioController : ControllerBase
{
    private readonly IAccesoUsuarioLN _accesoUsuarioLN;

    public AccesoUsuarioController(IAccesoUsuarioLN accesoUsuarioLN)
    {
        _accesoUsuarioLN = accesoUsuarioLN;
    }

    [AllowAnonymous]
    [HttpPost("IniciarSesion")]
    public async Task<IActionResult> IniciarSesion([FromBody] TInicioSesion datos)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _accesoUsuarioLN.IniciarSesionAsync(datos);
        if (resultado.Error == "No fue posible iniciar sesión.")
        {
            return StatusCode(StatusCodes.Status500InternalServerError, resultado);
        }

        if (!string.IsNullOrEmpty(resultado.Error))
        {
            return Unauthorized(resultado);
        }

        return Ok(resultado);
    }
}
