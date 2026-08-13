using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfazLN;

namespace Tienda.API.Controllers
{
    // Define la ruta base del controlador: api/ImagenProducto
    [Route("api/[controller]")]
    // Indica que es un controlador de API REST (habilita validación automática del modelo, inferencia de binding, etc.)
    [ApiController]
    public class ImagenProductoController : ControllerBase
    {
        private const long TamanoMaximoImagen = 5 * 1024 * 1024;

        private readonly IImagenProductoLN _imagenProductoLN;
        private readonly IWebHostEnvironment _entorno;
        private readonly ILogger<ImagenProductoController> _logger;

        public ImagenProductoController(
            IImagenProductoLN imagenProductoLN,
            IWebHostEnvironment entorno,
            ILogger<ImagenProductoController> logger)
        {
            _imagenProductoLN = imagenProductoLN;
            _entorno = entorno;
            _logger = logger;
        }

        // GET: api/ImagenProducto/ListarPorProducto/{productoId}
        // Devuelve las imágenes asociadas a un producto específico
        // Nota: este endpoint no tiene [ResponseCache], por lo que la respuesta podría cachearse
        [HttpGet("ListarPorProducto/{productoId}")]
        public async Task<IActionResult> ListarPorProducto(int productoId)
        {
            var resultado =
                await _imagenProductoLN.ListarPorProductoAsync(productoId);

            if (!string.IsNullOrEmpty(resultado.Error))
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }

        // POST: api/ImagenProducto/Insertar
        // Crea una nueva imagen de producto a partir de los datos enviados en el cuerpo de la petición
        // Nota: aquí no se valida ModelState.IsValid antes de insertar
        [HttpPost("Insertar")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Insertar(
            [FromBody] TImagenProducto datos)
        {
            // Envía la entidad a la capa de negocio para su inserción
            var resultado = await _imagenProductoLN.InsertarAsync(datos);

            if (!string.IsNullOrEmpty(resultado.Error))
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }

        //Recibe una imagen, la guarda y registra su ruta para el producto
        [HttpPost("Subir/{productoId:int}")]
        [Authorize(Roles = "Empleado")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(6 * 1024 * 1024)]
        [RequestFormLimits(
            MultipartBodyLengthLimit = 6 * 1024 * 1024)]
        public async Task<IActionResult> Subir(
            int productoId,
            IFormFile archivo)
        {
            if (productoId <= 0)
            {
                return BadRequest(new
                {
                    error = "El producto indicado no es válido."
                });
            }

            if (archivo is null || archivo.Length == 0)
            {
                return BadRequest(new
                {
                    error = "Debe seleccionar una imagen."
                });
            }

            if (archivo.Length > TamanoMaximoImagen)
            {
                return BadRequest(new
                {
                    error = "La imagen no puede superar los 5 MB."
                });
            }

            var extension =
                Path.GetExtension(archivo.FileName).ToLowerInvariant();

            var extensionesPermitidas = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase)
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".webp"
            };

            if (!extensionesPermitidas.Contains(extension))
            {
                return BadRequest(new
                {
                    error = "Solo se permiten imágenes JPG, JPEG, PNG o WEBP."
                });
            }

            //Comprueba el contenido real
            if (!await TieneFirmaValidaAsync(archivo, extension))
            {
                return BadRequest(new
                {
                    error = "El archivo seleccionado no es una imagen válida."
                });
            }

            string? rutaFisica = null;

            try
            {
                var raizPublica =
                    string.IsNullOrWhiteSpace(_entorno.WebRootPath)
                        ? Path.Combine(
                            _entorno.ContentRootPath,
                            "wwwroot")
                        : _entorno.WebRootPath;

                var carpetaImagenes = Path.Combine(
                    raizPublica,
                    "images",
                    "productos");

                Directory.CreateDirectory(carpetaImagenes);

                //Evita utilizar directamente el nombre enviado por el usuario
                var nombreArchivo =
                    $"{Guid.NewGuid():N}{extension}";

                rutaFisica = Path.Combine(
                    carpetaImagenes,
                    nombreArchivo);

                await using (var flujo = new FileStream(
                    rutaFisica,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
                {
                    await archivo.CopyToAsync(flujo);
                }

                var rutaPublica =
                    $"/images/productos/{nombreArchivo}";

                var resultado =
                    await _imagenProductoLN.InsertarAsync(
                        new TImagenProducto
                        {
                            ProductoId = productoId,
                            RutaImagen = rutaPublica
                        });

                if (!string.IsNullOrEmpty(resultado.Error) ||
                    resultado.Data is null)
                {
                    EliminarArchivoSiExiste(rutaFisica);
                    return BadRequest(resultado);
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                EliminarArchivoSiExiste(rutaFisica);

                _logger.LogError(
                    ex,
                    "Error al subir una imagen para el producto {ProductoId}.",
                    productoId);

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        error = "No fue posible guardar la imagen."
                    });
            }
        }

        [HttpDelete("Eliminar/{id}")]
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado =
                await _imagenProductoLN.EliminarAsync(
                    new TImagenProducto
                    {
                        ImagenId = id
                    });

            if (!string.IsNullOrEmpty(resultado.Error))
            {
                return BadRequest(resultado);
            }

            return Ok(resultado);
        }

        //Valida las firmas básicas de los formatos permitidos
        private static async Task<bool> TieneFirmaValidaAsync(
            IFormFile archivo,
            string extension)
        {
            var firma = new byte[12];

            await using var flujo = archivo.OpenReadStream();

            var bytesLeidos = await flujo.ReadAsync(
                firma.AsMemory(0, firma.Length));

            return extension switch
            {
                ".jpg" or ".jpeg" =>
                    bytesLeidos >= 3 &&
                    firma[0] == 0xFF &&
                    firma[1] == 0xD8 &&
                    firma[2] == 0xFF,

                ".png" =>
                    bytesLeidos >= 8 &&
                    firma[0] == 0x89 &&
                    firma[1] == 0x50 &&
                    firma[2] == 0x4E &&
                    firma[3] == 0x47 &&
                    firma[4] == 0x0D &&
                    firma[5] == 0x0A &&
                    firma[6] == 0x1A &&
                    firma[7] == 0x0A,

                ".webp" =>
                    bytesLeidos >= 12 &&
                    firma[0] == 0x52 &&
                    firma[1] == 0x49 &&
                    firma[2] == 0x46 &&
                    firma[3] == 0x46 &&
                    firma[8] == 0x57 &&
                    firma[9] == 0x45 &&
                    firma[10] == 0x42 &&
                    firma[11] == 0x50,

                _ => false
            };
        }

        //Limpia el archivo cuando no fue posible registrar su ruta
        private void EliminarArchivoSiExiste(string? rutaFisica)
        {
            if (string.IsNullOrWhiteSpace(rutaFisica) ||
                !System.IO.File.Exists(rutaFisica))
            {
                return;
            }

            try
            {
                System.IO.File.Delete(rutaFisica);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "No fue posible eliminar el archivo de imagen temporal.");
            }
        }
    }
}