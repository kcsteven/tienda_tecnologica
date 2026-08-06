export interface IRegistroCliente {
  tipoDocumentoId: number;
  numeroDocumento: string;
  nombre: string;
  apellido: string;
  fechaNacimiento?: string | null;
  telefono?: string | null;
  email?: string | null;
  nombreUsuario: string;
  contrasena: string;
  provincia: string;
  canton: string;
  distrito: string;
  senaExacta?: string | null;
}
