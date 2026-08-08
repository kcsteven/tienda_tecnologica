using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/Etiqueta
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class EtiquetaController : ControllerBase
    {
        // Dependencia hacia la capa de lógica de negocio (LN) de Etiqueta, inyectada por constructor
        private IEtiquetaLN _etiquetaLN { get; }

        // Constructor: recibe la implementación de IEtiquetaLN mediante inyección de dependencias
        public EtiquetaController(IEtiquetaLN etiquetaLN)
        {
            _etiquetaLN = etiquetaLN;
        }

        // GET: api/Etiqueta/Listar
        // Devuelve todas las etiquetas existentes
        [HttpGet("Listar")]
        // Evita que la respuesta se almacene en caché (ni en cliente ni en servidor)
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Listar()
        {
            // Llama a la capa de negocio para obtener el listado completo de etiquetas
            var resultado = await _etiquetaLN.ListarAsync();
            // Si el resultado trae un mensaje de error, responde con 400 Bad Request
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            // Caso contrario, responde 200 OK con los datos
            return Ok(resultado);
        }

        // GET: api/Etiqueta/Obtener/{id}
        // Obtiene una etiqueta puntual según su Id
        [HttpGet("Obtener/{id}")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Obtener(int id)
        {
            // Se construye un objeto TEtiqueta solo con el Id para buscarla en la capa de negocio
            var resultado = await _etiquetaLN.ObtenerAsync(new TEtiqueta { EtiquetaId = id });
            // Si hay error (por ejemplo, no se encontró), responde 404 Not Found
            if (!string.IsNullOrEmpty(resultado.Error)) return NotFound(resultado);
            return Ok(resultado);
        }

        // GET: api/Etiqueta/Buscar?nombre=...
        // Busca etiquetas cuyo nombre coincida (parcial o total) con el parámetro recibido
        [HttpGet("Buscar")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Buscar(string nombre)
        {
            // Construye el filtro de búsqueda con el nombre recibido por query string
            var resultado = await _etiquetaLN.BuscarAsync(new TEtiqueta { Nombre = nombre });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // POST: api/Etiqueta/Insertar
        // Crea una nueva etiqueta a partir de los datos enviados en el cuerpo de la petición
        [HttpPost("Insertar")]
        public async Task<IActionResult> Insertar([FromBody] TEtiqueta etiqueta)
        {
            // Valida el modelo recibido según las anotaciones de datos (DataAnnotations) definidas en TEtiqueta
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _etiquetaLN.InsertarAsync(etiqueta);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // PUT: api/Etiqueta/Modificar
        // Actualiza una etiqueta existente con los datos enviados en el cuerpo de la petición
        [HttpPut("Modificar")]
        public async Task<IActionResult> Modificar([FromBody] TEtiqueta etiqueta)
        {
            // Valida el modelo antes de procesar la modificación
            if (!ModelState.IsValid) return BadRequest(ModelState);
            // Envía la entidad a la capa de negocio para actualizarla
            var resultado = await _etiquetaLN.ModificarAsync(etiqueta);
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }

        // DELETE: api/Etiqueta/Eliminar/{id}
        // Elimina una etiqueta existente según su Id
        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            // Se construye un objeto TEtiqueta solo con el Id para indicar cuál eliminar
            var resultado = await _etiquetaLN.EliminarAsync(new TEtiqueta { EtiquetaId = id });
            if (!string.IsNullOrEmpty(resultado.Error)) return BadRequest(resultado);
            return Ok(resultado);
        }
    }
}