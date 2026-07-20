import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { IMarca } from '../model/IMarca';

const baseUrl = environment.baseUrl;

@Injectable({ providedIn: 'root' })
export class MarcaService {
  private http = inject(HttpClient);

  listar(): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Marca/Listar`);
  }
  obtener(id: number): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Marca/Obtener/${id}`);
  }
  buscar(nombre: string): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Marca/Buscar`, { params: { nombre } });
  }
  insertar(marca: IMarca): Observable<any> {
    return this.http.post<any>(`${baseUrl}/Marca/Insertar`, marca);
  }
  modificar(marca: IMarca): Observable<any> {
    return this.http.put<any>(`${baseUrl}/Marca/Modificar`, marca);
  }
  eliminar(id: number): Observable<any> {
    return this.http.delete<any>(`${baseUrl}/Marca/Eliminar/${id}`);
  }
}
