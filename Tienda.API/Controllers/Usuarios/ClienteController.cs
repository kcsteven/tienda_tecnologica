using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas.Usuarios;
using Tienda.Dominio.InterfazLN;
using Tienda.Dominio.InterfazLN.Usuarios;

//Permite que los empleados cambien el esdtado de los clientes
namespace Tienda.API.Controllers.Usuarios
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Empleado")]

    public class ClienteController : ControllerBase
    {
        private readonly IClienteLN _clienteLN;

        public ClienteController(IClienteLN clienteLN)
        {
            _clienteLN = clienteLN;
        }

        [HttpGet("ListarAdministracion")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        //Devuelve los clientes activos/inactivos
        public async Task<IActionResult> ListarAdministracion()
        {
            var resultado = await _clienteLN.ListarAdministracionAsync();

            if (!string.IsNullOrEmpty(resultado.Error))
            {
                return BadRequest(resultado.Error);
            }

            return Ok(resultado);

        }

        //Activa/desactiva el acceso de un cliente
        [HttpPut("CambiarEstado")]
        public async Task<IActionResult> CambiarEstado(
            [FromBody] TCambiarEstadoCliente datos)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultado = await _clienteLN.CambiarEstadoAsync(datos);

            if(!string.IsNullOrEmpty(resultado.Error))
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);

        }

    }
}
