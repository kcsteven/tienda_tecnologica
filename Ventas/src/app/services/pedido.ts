import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { IPedido } from '../model/IPedido';

const baseUrl = environment.baseUrl;

@Injectable({
  providedIn: 'root',
})

export class PedidoService {
  private http = inject(HttpClient);

  listar(): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Pedido/Listar`);
  }

  obtener(id: number): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Pedido/Obtener/${id}`);
  }

  buscar(nombrePedido: string): Observable<any> {
    return this.http.get<any>(
      `${baseUrl}/Pedido/Buscar`,
      {
        params: {
          nombrePedido
        }
      }
    );
  }

  insertar(pedido: IPedido): Observable<any> {
    return this.http.post<any>(
      `${baseUrl}/Pedido/Insertar`,
      pedido
    );
  }

  modificar(pedido: IPedido): Observable<any> {
    return this.http.put<any>(
      `${baseUrl}/Pedido/Modificar`,
      pedido
    );
  }

  eliminar(id: number): Observable<any> {
    return this.http.delete<any>(
      `${baseUrl}/Pedido/Eliminar/${id}`
    );
  }
}
