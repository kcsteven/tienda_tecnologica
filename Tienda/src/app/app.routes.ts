import { Routes } from '@angular/router';

import { DashboardPrincipal } from '../Componentes/dashboard-principal/dashboard-principal';
import { PrincipalComponent } from '../Componentes/principal/principal';
import { ClienteComponent } from '../Componentes/cliente/cliente';
import { ProductoComponent } from '../Componentes/producto/producto';
import { CategoriaComponent } from '../Componentes/categoria/categoria';

import { TiendaLayoutComponent } from '../Componentes/tienda/tienda-layout/tienda-layout';
import { TiendaHomeComponent } from '../Componentes/tienda/tienda-home/tienda-home';
import { TiendaProductoListadoComponent } from '../Componentes/tienda/tienda-producto-listado/tienda-producto-listado';
import { TiendaProductoDetalleComponent } from '../Componentes/tienda/tienda-producto-detalle/tienda-producto-detalle';

export const routes: Routes = [

  {
    path: '',
    component: TiendaLayoutComponent,
    children: [
      { path: '', component: TiendaHomeComponent },
      { path: 'categoria/:id', component: TiendaProductoListadoComponent, data: { tipo: 'categoria' } },
      { path: 'subcategoria/:id', component: TiendaProductoListadoComponent, data: { tipo: 'subcategoria' } },
      { path: 'producto/:id', component: TiendaProductoDetalleComponent }
    ]
  },

  {
    path: 'admin',
    component: DashboardPrincipal,
    children: [
      { path: '', component: PrincipalComponent },
      { path: 'cliente', component: ClienteComponent },
      { path: 'producto', component: ProductoComponent },
      { path: 'categoria', component: CategoriaComponent }
    ]
  }
];
