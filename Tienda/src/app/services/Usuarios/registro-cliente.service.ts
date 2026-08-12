import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { IRegistroCliente } from '../../model/Usuarios/registro-cliente';
import { ITipoDocumento } from '../../model/Usuarios/tipo-documento';

interface IRespuestaApi<T> {
  data?: T;
  error?: string;
}

const baseUrl = environment.baseUrl;

@Injectable({ providedIn: 'root' })
export class RegistroClienteService {
  private http = inject(HttpClient);

  listarTiposDocumento(): Observable<IRespuestaApi<ITipoDocumento[]>> {

    return this.http.get<IRespuestaApi<ITipoDocumento[]>>(
      `${baseUrl}/RegistroCliente/TiposDocumento`
    );
  }

  registrar(registro: IRegistroCliente): Observable<IRespuestaApi<boolean>> {

    return this.http.post<IRespuestaApi<boolean>>(
      `${baseUrl}/RegistroCliente/Registrar`,
      registro
    );
  }
}
