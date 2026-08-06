import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { IResena } from '../model/IResena';

const baseUrl = environment.baseUrl;

@Injectable({ providedIn: 'root' })
export class ResenaService {
  private http = inject(HttpClient);

  listarPorProducto(productoId: number): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Resena/ListarPorProducto/${productoId}`);
  }

  insertar(resena: Partial<IResena>): Observable<any> {
    return this.http.post<any>(`${baseUrl}/Resena/Insertar`, resena);
  }
}
