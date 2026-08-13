import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { IAjustarInventario } from '../model/IAjustarInventario';

const baseUrl = environment.baseUrl;

@Injectable({ providedIn: 'root' })
export class InventarioService {
  private http = inject(HttpClient);

  listar(): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Inventario/Listar`);
  }
  listarPorProducto(productoId: number): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Inventario/ListarPorProducto/${productoId}`);
  }

  modificar(ajuste: IAjustarInventario): Observable<any> {
    return this.http.put<any>(`${baseUrl}/Inventario/Modificar`, ajuste);
  }
}
