import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccesoUsuarioService } from '../services/Usuarios/acceso-usuario.service';
//Permite que solamente los empleados puedan entrar al area de administracion
export const empleadoGuard: CanActivateFn = () => {
  const accesoUsuarioService = inject(AccesoUsuarioService);
  const router = inject(Router);
  const sesion = accesoUsuarioService.obtenerSesion();

  if (!sesion) {
    return router.createUrlTree(['/iniciar-sesion']);
  }

  if (sesion.rol !== 'Empleado') {
    return router.createUrlTree(['/']);
  }

  return true;
};
