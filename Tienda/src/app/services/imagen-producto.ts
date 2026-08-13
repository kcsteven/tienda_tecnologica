import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment.development';

const baseUrl = environment.baseUrl;

@Injectable({ providedIn: 'root' })
export class ImagenProductoService {
  private http = inject(HttpClient);


  //Obtiene las imagenes guardadas para el producto
  listarPorProducto(productoId: number): Observable<any> {
    return this.http.get<any>(`${baseUrl}/ImagenProducto/ListarPorProducto/${productoId}`);
  }


//Envia la imagen para que la API la guarde
subir(productoId: number, archivo: File): Observable<any> {
 const datos = new FormData();

 datos.append('archivo', archivo);

 return this.http.post<any>(

   baseUrl + '/ImagenProducto/Subir/' + productoId,
   datos
 );
}
}
