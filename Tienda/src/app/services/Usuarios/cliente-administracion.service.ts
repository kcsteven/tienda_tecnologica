import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import {ICambiarEstadoCliente,
        IClienteAdministracion
} from '../../model/Usuarios/cliente-administracion';
import { IRespuestaApi } from '../../model/Usuarios/acceso-usuario';

const baseUrl = environment.baseUrl;


@Injectable({ providedIn: 'root' })
export class ClienteAdministracionService {
  private http = inject(HttpClient);

  //Obtiene clientes activos/inactivos para la administración
  listar(): Observable<IRespuestaApi<IClienteAdministracion[]>> {
    return this.http.get<IRespuestaApi<IClienteAdministracion[]>>(
      `${baseUrl}/Cliente/ListarAdministracion`
    );
  }

  //Solicita el cambio de estado del cliente
  cambiarEstado(
    datos: ICambiarEstadoCliente
  ): Observable<IRespuestaApi<boolean>> {

    return this.http.put<IRespuestaApi<boolean>>(
      `${baseUrl}/Cliente/CambiarEstado`,
      datos
    );

  }


}
