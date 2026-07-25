export interface IProveedor {
  proveedorId: number;
  nombre: string;
  telefono?: string;
  email?: string;
  activo: boolean;
  creadoEn: Date;
  creadoPor: string;
  actualizadoEn?: Date;
  actualizadoPor?: string;
}
