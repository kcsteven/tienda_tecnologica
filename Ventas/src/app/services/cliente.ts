import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { ICliente } from '../model/ICliente';

const baseUrl = environment.baseUrl;

@Injectable({
  providedIn: 'root',
})

export class CategoriaService {
  private http = inject(HttpClient);

  listar(): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Cliente/Listar`);
  }

  obtener(id: number): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Cliente/Obtener/${id}`);
  }

  buscar(nombreCategoria: string): Observable<any> {
    return this.http.get<any>(
      `${baseUrl}/Cliente/Buscar`,
      {
        params: {
          nombreCategoria
        }
      }
    );
  }

  insertar(categoria: ICliente): Observable<any> {
    return this.http.post<any>(
      `${baseUrl}/Cliente/Insertar`,
      categoria
    );
  }
  modificar(categoria: ICliente): Observable<any> {
    return this.http.put<any>(
      `${baseUrl}/Cliente/Modificar`,
      categoria
    );
  }
  eliminar(id: number): Observable<any> {
    return this.http.delete<any>(
      `${baseUrl}/Cliente/Eliminar/${id}`
    );
  }
}
