import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { AccesoUsuarioService } from '../services/Usuarios/acceso-usuario.service';

const apiBaseUrl = environment.baseUrl.replace(/\/$/, '');

export const autenticacionInterceptor: HttpInterceptorFn = (request, next) => {
  const accesoUsuarioService = inject(AccesoUsuarioService);
  const sesion = accesoUsuarioService.obtenerSesion();
  const esSolicitudApi =
    request.url === apiBaseUrl || request.url.startsWith(`${apiBaseUrl}/`);

  if (!sesion?.token || !esSolicitudApi) {
    return next(request);
  }

  return next(
    request.clone({
      setHeaders: {
        Authorization: `Bearer ${sesion.token}`
      }
    })
  );
};
