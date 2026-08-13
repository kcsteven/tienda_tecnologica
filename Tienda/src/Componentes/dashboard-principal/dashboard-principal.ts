import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { NgScrollbar } from 'ngx-scrollbar';

import {
  AvatarComponent,
  ShadowOnScrollDirective,
  SidebarBrandComponent,
  SidebarComponent,
  SidebarFooterComponent,
  SidebarHeaderComponent,
  SidebarNavComponent,
  SidebarToggleDirective,
  SidebarTogglerDirective,
  INavData
} from '@coreui/angular';

import { DefaultHeader } from '../defauld-header/defauld-header';
import { DefaultFooterComponent } from '../defauld-footer/defauld-footer';

function isOverflown(element: HTMLElement) {
  return (
    element.scrollHeight > element.clientHeight ||
    element.scrollWidth > element.clientWidth
  );
}

export const navItems: INavData[] = [
  {
    name: 'Dashboard',
    url: '/',
    iconComponent: { name: 'cil-speedometer' },
    badge: {
      color: 'info',
      text: 'NEW'
    }
  },
  {
    name: 'Cliente',
    url: '/cliente',
    iconComponent: { name: 'cil-user' }
  },
  {
    name: 'Producto',
    url: '/admin/producto',
    iconComponent: { name: 'cil-cart' }
  },
  {
    name: 'Categoría',
    url: '/categoria',
    iconComponent: { name: 'cil-tags' }
  }
];

@Component({
  selector: 'app-dashboard-principal',
  standalone: true,
  imports: [
    RouterOutlet,
    RouterLink,
    NgScrollbar,
    SidebarComponent,
    SidebarHeaderComponent,
    SidebarBrandComponent,
    SidebarNavComponent,
    SidebarFooterComponent,
    SidebarToggleDirective,
    SidebarTogglerDirective,
    ShadowOnScrollDirective,
    AvatarComponent,
    DefaultHeader,
    DefaultFooterComponent
  ],
  templateUrl: './dashboard-principal.html',
  styleUrl: './dashboard-principal.css',
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class DashboardPrincipal {
  navItems = navItems;
}
