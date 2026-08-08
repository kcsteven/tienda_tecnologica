import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TiendaHeaderComponent } from '../tienda-header/tienda-header';

@Component({
  selector: 'app-tienda-layout',
  standalone: true,
  imports: [RouterOutlet, TiendaHeaderComponent],
  templateUrl: './tienda-layout.html'
})
// Componente de layout: envuelve el header y el contenido de las páginas de la tienda
export class TiendaLayoutComponent { }
