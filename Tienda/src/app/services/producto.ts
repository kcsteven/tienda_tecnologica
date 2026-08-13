import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { IProducto } from '../model/IProducto';
import { ICrearProductoConInventario } from '../model/ICrearProductoConInventario';
import { IActualizarProducto } from '../model/IActualizarProducto';
import { ICambiarEstadoProducto } from '../model/ICambiarEstadoProducto';

const baseUrl = environment.baseUrl;

@Injectable({
  providedIn: 'root',
})

export class ProductoService {
  private http = inject(HttpClient);

  listar(): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Producto/Listar`);
  }

  listarAdministracion(): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Producto/ListarAdministracion`);
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

  insertar(producto: ICrearProductoConInventario): Observable<any> {
    return this.http.post<any>(
      `${baseUrl}/Producto/Insertar`,
      producto
    );
  }

  modificar(producto: IActualizarProducto): Observable<any> {
    return this.http.put<any>(
      `${baseUrl}/Producto/Modificar`,
      producto
    );
  }

  cambiarEstado(producto: ICambiarEstadoProducto): Observable<any> {
    return this.http.put<any>(
      `${baseUrl}/Producto/CambiarEstado`,
      producto
    );
  }
}
