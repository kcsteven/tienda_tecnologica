using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Tienda.Dominio.Entidades;
using Tienda.Dominio.EntidadesTipadas;
using Tienda.Dominio.InterfacesAD;
using Tienda.Dominio.InterfazLN;
using Tienda.Utilidades;

namespace Tienda.LogicaNegocio.Implementaciones
{
    // Implementación de la lógica de negocio (LN) para la entidad Proveedor
    public class ProveedorLN : IProveedorLN
    {
        // Unidad de trabajo (Entity Framework) para acceder a los repositorios de datos
        private IUnidadTrabajoEF _unidadDeTrabajo { get; set; }
        // Logger para registrar errores y eventos de esta clase
        private ILogger<ProveedorLN> _logger { get; }
        // AutoMapper para convertir entre entidades de dominio (Proveedor) y entidades tipadas (TProveedor)
        private readonly IMapper _mapper;

        // Constructor: recibe las dependencias mediante inyección de dependencias
        public ProveedorLN(IUnidadTrabajoEF unidadTrabajo, ILogger<ProveedorLN> logger, IMapper mapper)
        {
            _unidadDeTrabajo = unidadTrabajo;
            _logger = logger;
            _mapper = mapper;
        }

        // Inserta un nuevo proveedor, validando primero que no exista ya uno con el mismo nombre
        public async Task<Respuesta<TProveedor>> InsertarAsync(TProveedor datos)
        {
            var resultado = new Respuesta<TProveedor>();
            try
            {
                // Verifica si ya existe un proveedor registrado con el mismo nombre
                var existente = await _unidadDeTrabajo.TProveedor.ObtenerEntidadAsync(x => x.Nombre == datos.Nombre);
                if (existente.Data != null)
                {
                    resultado.Error = "Ya existe un proveedor registrado con ese nombre.";
                    return resultado;
                }

                // Marca la fecha de creación en UTC
                datos.CreadoEn = DateTime.UtcNow;
                // Convierte el DTO tipado a la entidad de dominio
                var entidad = _mapper.Map<Proveedor>(datos);
                // Inserta la entidad en el repositorio
                var respuesta = await _unidadDeTrabajo.TProveedor.InsertarAsync(entidad);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                // Convierte la entidad insertada de vuelta a DTO tipado para la respuesta
                resultado.Data = _mapper.Map<TProveedor>(respuesta.Data);
            }
            catch (Exception ex)
            {
                // Registra el error y lo devuelve en la respuesta
                _logger.LogError(ex, "Error al insertar proveedor {Nombre}", datos.Nombre);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Modifica un proveedor existente, validando primero que exista
        public async Task<Respuesta<TProveedor>> ModificarAsync(TProveedor datos)
        {
            var resultado = new Respuesta<TProveedor>();
            try
            {
                // Busca el proveedor actual en base de datos por su Id
                var actual = await _unidadDeTrabajo.TProveedor.ObtenerEntidadAsync(x => x.ProveedorId == datos.ProveedorId);
                if (actual.Data == null)
                {
                    resultado.Error = "No existe el proveedor a modificar.";
                    return resultado;
                }

                // Marca la fecha de actualización en UTC
                datos.ActualizadoEn = DateTime.UtcNow;
                // Copia los valores del DTO recibido sobre la entidad existente rastreada por EF
                _mapper.Map(datos, actual.Data);

                // Guarda los cambios en el repositorio
                var respuesta = await _unidadDeTrabajo.TProveedor.ModificarAsync(actual.Data);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                // Convierte la entidad modificada de vuelta a DTO tipado para la respuesta
                resultado.Data = _mapper.Map<TProveedor>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar ProveedorId {ProveedorId}", datos.ProveedorId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Elimina un proveedor existente, validando primero que exista
        public async Task<Respuesta<bool>> EliminarAsync(TProveedor datos)
        {
            var resultado = new Respuesta<bool>();
            try
            {
                // Busca el proveedor a eliminar por su Id
                var proveedor = await _unidadDeTrabajo.TProveedor.ObtenerEntidadAsync(x => x.ProveedorId == datos.ProveedorId);
                if (proveedor.Data == null)
                {
                    resultado.Error = "No existe el proveedor a eliminar.";
                    return resultado;
                }

                // Elimina la entidad del repositorio
                var respuesta = await _unidadDeTrabajo.TProveedor.EliminarAsync(proveedor.Data);
                // Confirma (commit) los cambios en la unidad de trabajo
                _unidadDeTrabajo.Completar();

                resultado.Data = respuesta.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar ProveedorId {ProveedorId}", datos.ProveedorId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Lista todos los proveedores existentes
        // Nota: a diferencia de otros métodos Listar, aquí no se revisa "resp.Error" antes de mapear los datos
        public async Task<Respuesta<IEnumerable<TProveedor>>> ListarAsync()
        {
            var resultado = new Respuesta<IEnumerable<TProveedor>>();
            try
            {
                // Obtiene todos los proveedores desde el repositorio
                var resp = await _unidadDeTrabajo.TProveedor.ListarAsync();
                // Convierte la lista de entidades de dominio a DTOs tipados
                resultado.Data = _mapper.Map<IEnumerable<TProveedor>>(resp.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar proveedores.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Busca proveedores cuyo nombre contenga el texto recibido
        // Nota: aquí tampoco se revisa "respuesta.Error" antes de mapear los datos
        public async Task<Respuesta<IEnumerable<TProveedor>>> BuscarAsync(TProveedor datos)
        {
            var resultado = new Respuesta<IEnumerable<TProveedor>>();
            try
            {
                // Filtra los proveedores cuyo nombre contenga el texto de búsqueda
                var respuesta = await _unidadDeTrabajo.TProveedor.BuscarAsync(x => x.Nombre.Contains(datos.Nombre));
                resultado.Data = _mapper.Map<IEnumerable<TProveedor>>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar proveedores.");
                resultado.Error = ex.Message;
            }
            return resultado;
        }

        // Obtiene un proveedor puntual según su Id
        public async Task<Respuesta<TProveedor>> ObtenerAsync(TProveedor datos)
        {
            var resultado = new Respuesta<TProveedor>();
            try
            {
                // Busca el proveedor por su Id
                var respuesta = await _unidadDeTrabajo.TProveedor.ObtenerEntidadAsync(x => x.ProveedorId == datos.ProveedorId);
                if (respuesta.Data == null)
                {
                    resultado.Error = "Proveedor no encontrado.";
                    return resultado;
                }
                resultado.Data = _mapper.Map<TProveedor>(respuesta.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ProveedorId {ProveedorId}", datos.ProveedorId);
                resultado.Error = ex.Message;
            }
            return resultado;
        }
    }
}