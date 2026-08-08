import { environment } from '../../environments/environment.development';

// Convierte una ruta relativa de imagen en la URL completa hacia el backend
export function urlImagen(rutaRelativa?: string): string {
  // Si no hay ruta, devuelve vacío (no hay imagen)
  if (!rutaRelativa) return '';
  // Quita el '/api' de la URL base para obtener el origen del servidor
  const origenApi = environment.baseUrl.replace('/api', '');
  // Une el origen con la ruta relativa de la imagen
  return `${origenApi}/${rutaRelativa}`;
}
