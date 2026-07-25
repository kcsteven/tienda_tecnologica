import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { ISubcategoria } from '../model/ISubcategoria';

const baseUrl = environment.baseUrl;

@Injectable({ providedIn: 'root' })
export class SubcategoriaService {
  private http = inject(HttpClient);

  listar(): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Subcategoria/Listar`);
  }
  listarPorCategoria(categoriaId: number): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Subcategoria/ListarPorCategoria/${categoriaId}`);
  }
  obtener(id: number): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Subcategoria/Obtener/${id}`);
  }
  buscar(nombre: string): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Subcategoria/Buscar`, { params: { nombre } });
  }
  insertar(subcategoria: ISubcategoria): Observable<any> {
    return this.http.post<any>(`${baseUrl}/Subcategoria/Insertar`, subcategoria);
  }
  modificar(subcategoria: ISubcategoria): Observable<any> {
    return this.http.put<any>(`${baseUrl}/Subcategoria/Modificar`, subcategoria);
  }
  eliminar(id: number): Observable<any> {
    return this.http.delete<any>(`${baseUrl}/Subcategoria/Eliminar/${id}`);
  }
}
