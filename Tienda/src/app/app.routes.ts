import { Routes } from '@angular/router';

import { DashboardPrincipal } from '../Componentes/dashboard-principal/dashboard-principal';
import { PrincipalComponent } from '../Componentes/principal/principal';
import { ClienteComponent } from '../Componentes/cliente/cliente';
import { ProductoComponent } from '../Componentes/producto/producto';
import { CategoriaComponent } from '../Componentes/categoria/categoria';

import { TiendaLayoutComponent } from '../Componentes/tienda/tienda-layout/tienda-layout';
import { TiendaHomeComponent } from '../Componentes/tienda/tienda-home/tienda-home';
import { TiendaProductoListadoComponent } from '../Componentes/tienda/tienda-producto-listado/tienda-producto-listado';
import { RegistroClienteComponent } from '../Componentes/Usuarios/registro-cliente';
import { InicioSesionComponent } from '../Componentes/Usuarios/inicio-sesion';
import { TiendaProductoDetalleComponent } from '../Componentes/tienda/tienda-producto-detalle/tienda-producto-detalle';
import { TiendaCarritoComponent } from '../Componentes/tienda/tienda-carrito/tienda-carrito';
import { empleadoGuard } from './seguridad/empleado.guard';
import { TiendaPedidoConfirmacionComponent } from '../Componentes/tienda/tienda-pedido-confirmacion/tienda-pedido-confirmacion';
import { TiendaFavoritosComponent } from '../Componentes/tienda/tienda-favoritos/tienda-favoritos';

export const routes: Routes = [

  {
    path: '',
    component: TiendaLayoutComponent,
    children: [
      { path: '', component: TiendaHomeComponent },
      { path: 'categoria/:id', component: TiendaProductoListadoComponent, data: { tipo: 'categoria' } },
      { path: 'buscar', component: TiendaProductoListadoComponent, data: { tipo: 'busqueda' } },
      { path: 'subcategoria/:id', component: TiendaProductoListadoComponent, data: { tipo: 'subcategoria' } },
      { path: 'producto/:id', component: TiendaProductoDetalleComponent },
      { path: 'carrito', component: TiendaCarritoComponent },
      { path: 'pedido/:id', component: TiendaPedidoConfirmacionComponent },
      { path: 'registro', component: RegistroClienteComponent },
      { path: 'favoritos', component: TiendaFavoritosComponent },
      { path: 'iniciar-sesion', component: InicioSesionComponent }

    ]
  },

  {
    path: 'admin',
    component: DashboardPrincipal,
    canActivate: [empleadoGuard],
    children: [
      { path: '', component: PrincipalComponent },
      { path: 'cliente', component: ClienteComponent },
      { path: 'producto', component: ProductoComponent },
      { path: 'categoria', component: CategoriaComponent }
    ]
  }
];
