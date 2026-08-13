export interface IClienteAdministracion {
  clienteId: number;
  usuarioId: number;
  nombreCompleto: string;
  numeroDocumento: string;
  email: string | null;
  nombreUsuario: string;
  fechaRegistro: string;
  activo: boolean;

}


export interface ICambiarEstadoCliente {
  clienteId: number;
  activo: boolean;

}
