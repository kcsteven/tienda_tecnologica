import { environment } from '../../environments/environment.development';

export function urlImagen(rutaRelativa?: string): string {
  if (!rutaRelativa) return '';
  const origenApi = environment.baseUrl.replace('/api', '');
  return `${origenApi}/${rutaRelativa}`;
}
