import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

const baseUrl = environment.baseUrl;

@Injectable({ providedIn: 'root' })
export class ProductoGarantiaService {
  private http = inject(HttpClient);

  listarPorProducto(productoId: number): Observable<any> {
    return this.http.get<any>(`${baseUrl}/ProductoGarantia/ListarPorProducto/${productoId}`);
  }
}
