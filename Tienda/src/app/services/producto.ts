import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { IProducto } from '../model/IProducto';

const baseUrl = environment.baseUrl;

@Injectable({
  providedIn: 'root',
})

export class ProductoService {
  private http = inject(HttpClient);

  listar(): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Producto/Listar`);
  }

  obtener(id: number): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Producto/Obtener/${id}`);
  }

  buscar(nombreProducto: string): Observable<any> {
    return this.http.get<any>(
      `${baseUrl}/Producto/Buscar`,
      {
        params: {
          nombreProducto
        }
      }
    );
  }

  insertar(producto: IProducto): Observable<any> {
    return this.http.post<any>(
      `${baseUrl}/Producto/Insertar`,
      producto
    );
  }

  modificar(producto: IProducto): Observable<any> {
    return this.http.put<any>(
      `${baseUrl}/Producto/Modificar`,
      producto
    );
  }

  eliminar(id: number): Observable<any> {
    return this.http.delete<any>(
      `${baseUrl}/Producto/Eliminar/${id}`
    );
  }
}
