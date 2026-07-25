import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { IProveedor } from '../model/IProveedor';

const baseUrl = environment.baseUrl;

@Injectable({ providedIn: 'root' })
export class ProveedorService {
  private http = inject(HttpClient);

  listar(): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Proveedor/Listar`);
  }
  obtener(id: number): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Proveedor/Obtener/${id}`);
  }
  buscar(nombre: string): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Proveedor/Buscar`, { params: { nombre } });
  }
  insertar(proveedor: IProveedor): Observable<any> {
    return this.http.post<any>(`${baseUrl}/Proveedor/Insertar`, proveedor);
  }
  modificar(proveedor: IProveedor): Observable<any> {
    return this.http.put<any>(`${baseUrl}/Proveedor/Modificar`, proveedor);
  }
  eliminar(id: number): Observable<any> {
    return this.http.delete<any>(`${baseUrl}/Proveedor/Eliminar/${id}`);
  }
}
