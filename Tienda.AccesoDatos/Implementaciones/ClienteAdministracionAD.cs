using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Tienda.AccesoDatos.Contexto;
using Tienda.Dominio.EntidadesTipadas.Usuarios;
using Tienda.Dominio.InterfacesAD;

//Consulta clientes sin exponer credenciales y solo cambia el estado
namespace Tienda.AccesoDatos.Implementaciones
{
    public class ClienteAdministracionAD : IClienteAdministracionAD
    {
        private readonly VentasContext _contexto;

        public ClienteAdministracionAD(VentasContext contexto)
        {
            _contexto = contexto;
        }

        // Obtiene todos los clientes activos/inactivos
        public async Task<IReadOnlyList<TClienteAdministracion>> ListarAsync()
        {
            return await CrearConsultaClientes()
                .OrderBy(cliente => cliente.NombreCompleto)
                .ToListAsync();
        }

        // Busca un cliente que tenga el rol Cliente
        public async Task<TClienteAdministracion?> ObtenerAsync(int clienteId)
        {
            return await CrearConsultaClientes()
                .SingleOrDefaultAsync(cliente => cliente.ClienteId == clienteId);
        }

        //Actualiza directamente Usuario.Activo sin cargar la contraseña
        public async Task<int> CambiarEstadoAsync(int clienteId, bool activo)
        {
            return await _contexto.Usuarios
                .Where(usuario =>
                    usuario.Rol.Nombre == "Cliente" &&
                    _contexto.Clientes.Any(cliente =>
                        cliente.ClienteId == clienteId &&
                        cliente.PersonaId == usuario.PersonaId))
                .ExecuteUpdateAsync(actualizacion =>
                    actualizacion.SetProperty(usuario => usuario.Activo, activo));
        }

        //Muestra únicamente los datos permitidos para la administración
        private IQueryable<TClienteAdministracion> CrearConsultaClientes()
        {
            return
                from cliente in _contexto.Clientes.AsNoTracking()
                join persona in _contexto.Personas.AsNoTracking()
                    on cliente.PersonaId equals persona.PersonaId
                join usuario in _contexto.Usuarios.AsNoTracking()
                    on persona.PersonaId equals usuario.PersonaId
                join rol in _contexto.Roles.AsNoTracking()
                    on usuario.RolId equals rol.RolId
                where rol.Nombre == "Cliente"
                select new TClienteAdministracion
                {
                    ClienteId = cliente.ClienteId,
                    UsuarioId = usuario.UsuarioId,
                    NombreCompleto = (persona.Nombre + " " + persona.Apellido).Trim(),
                    NumeroDocumento = persona.NumeroDocumento,
                    Email = persona.Email,
                    NombreUsuario = usuario.NombreUsuario,
                    FechaRegistro = cliente.FechaRegistro,
                    Activo = usuario.Activo
                };
        }
    }
}
