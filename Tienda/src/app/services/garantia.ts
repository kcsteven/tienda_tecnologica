import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

const baseUrl = environment.baseUrl;

@Injectable({ providedIn: 'root' })
export class GarantiaService {
  private http = inject(HttpClient);

  obtener(id: number): Observable<any> {
    return this.http.get<any>(`${baseUrl}/Garantia/Obtener/${id}`);
  }
}
