export interface IInicioSesion {
  email: string;
  contrasena: string;
}

export interface ISesionUsuario {

  usuarioId: number;

  personaId: number;

  clienteId?: number;

  nombreUsuario: string;

  nombreCompleto: string;

  email: string;

  rol: string;

  token: string;

  expiraEnUtc: string;

}

export interface IRespuestaApi<T> {

  data?: T;

  error?: string;

}
