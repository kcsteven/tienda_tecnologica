import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { IInicioSesion, IRespuestaApi, ISesionUsuario } from '../../model/Usuarios/acceso-usuario';

const baseUrl = environment.baseUrl;
const claveSesion = 'tienda.sesion';

@Injectable({ providedIn: 'root' })
export class AccesoUsuarioService {


  private http = inject(HttpClient);
  private sesionSignal = signal<ISesionUsuario | null>(this.leerSesion());

  readonly sesion = this.sesionSignal.asReadonly();

  iniciarSesion(datos: IInicioSesion): Observable<IRespuestaApi<ISesionUsuario>> {


    return this.http.post<IRespuestaApi<ISesionUsuario>>(
      `${baseUrl}/AccesoUsuario/IniciarSesion`,
      datos


    ).pipe(
      tap(respuesta => {


        if (respuesta.data) {
          this.guardarSesion(respuesta.data);

        }
      })
    );
  }

  guardarSesion(sesion: ISesionUsuario): void {

    sessionStorage.setItem(claveSesion, JSON.stringify(sesion));
    this.sesionSignal.set(sesion);
  }

  obtenerSesion(): ISesionUsuario | null {

    return this.sesionSignal();
  }

  cerrarSesion(): void {

    sessionStorage.clear();
    this.sesionSignal.set(null);

  }

  private leerSesion(): ISesionUsuario | null {

    const sesionAlmacenada = sessionStorage.getItem(claveSesion);

    if (!sesionAlmacenada) {

      return null;

    }

    try {
      return JSON.parse(sesionAlmacenada) as ISesionUsuario;

    } catch {

      sessionStorage.removeItem(claveSesion);

      return null;
    }
  }
}
