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


  //Envia las credenciales y guarda la sesion
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


  //guarda la sesion en sessionstorage y actualiza el estado
  guardarSesion(sesion: ISesionUsuario): void {

    sessionStorage.setItem(claveSesion, JSON.stringify(sesion));
    this.sesionSignal.set(sesion);
  }


  //Devuelve la sesion que se esta utilizando
  obtenerSesion(): ISesionUsuario | null {

    return this.sesionSignal();
  }


  //Elimina los datos locales y finaliza la sesion
  cerrarSesion(): void {

    sessionStorage.clear();
    this.sesionSignal.set(null);

  }


  //Recupera una sesion almacenada
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
