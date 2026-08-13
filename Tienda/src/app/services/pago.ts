import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';
import { IPago } from '../model/IPago';

const baseUrl = environment.baseUrl;

@Injectable({
  providedIn: 'root'
})
export class PagoService {

  private http = inject(HttpClient);

  insertar(pago: IPago): Observable<any> {
    return this.http.post<any>(
      `${baseUrl}/Pago/Insertar`,
      pago
    );
  }

  listarPorPedido(pedidoId: number): Observable<any> {
    return this.http.get<any>(
      `${baseUrl}/Pago/ListarPorPedido/${pedidoId}`
    );
  }
}
