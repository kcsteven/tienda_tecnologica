
import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { ICategoria } from '../model/ICategoria';

const baseUrl = environment.baseUrl;

@Injectable({ providedIn: 'root' })
export class CategoriaService {
  private http = inject(HttpClient);

  listar(): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Categoria/Listar`);
  }
  obtener(id: number): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Categoria/Obtener/${id}`);
  }
  buscar(nombre: string): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Categoria/Buscar`, { params: { nombre } });
  }
  insertar(categoria: ICategoria): Observable<any> {
    return this.http.post<any>(`${baseUrl}/Categoria/Insertar`, categoria);
  }
  modificar(categoria: ICategoria): Observable<any> {
    return this.http.put<any>(`${baseUrl}/Categoria/Modificar`, categoria);
  }
  eliminar(id: number): Observable<any> {
    return this.http.delete<any>(`${baseUrl}/Categoria/Eliminar/${id}`);
  }
}
