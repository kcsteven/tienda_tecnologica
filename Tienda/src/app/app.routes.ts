import { Routes } from '@angular/router';

import { DashboardPrincipal } from '../Componentes/dashboard-principal/dashboard-principal';
import { PrincipalComponent } from '../Componentes/principal/principal';
import { ClienteComponent } from '../Componentes/cliente/cliente';
import { ProductoComponent } from '../Componentes/producto/producto';
import { CategoriaComponent } from '../Componentes/categoria/categoria';

export const routes: Routes = [
  {
    path: '',
    component: DashboardPrincipal,
    children: [
      {
        path: '',
        component: PrincipalComponent
      },
      {
        path: 'cliente',
        component: ClienteComponent
      },
      {
        path: 'producto',
        component: ProductoComponent
      },
      {
        path: 'categoria',
        component: CategoriaComponent
      }
    ]
  }
];
