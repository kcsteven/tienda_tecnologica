import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CategoriaService } from '../../../app/services/categoria';
import { SubcategoriaService } from '../../../app/services/subcategoria';
import { ICategoria } from '../../../app/model/ICategoria';
import { ISubcategoria } from '../../../app/model/ISubcategoria';
import { CarritoService } from '../../../app/services/carrito';
import { AccesoUsuarioService } from '../../../app/services/Usuarios/acceso-usuario.service';
import { FavoritosService } from '../../../app/services/favoritos';

interface CategoriaConSubs extends ICategoria {
  subcategorias: ISubcategoria[];
}

@Component({
  selector: 'app-tienda-header',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './tienda-header.html',
  styleUrl: './tienda-header.scss'
})
export class TiendaHeaderComponent implements OnInit {
  private categoriaService = inject(CategoriaService);
  private subcategoriaService = inject(SubcategoriaService);
  carritoService = inject(CarritoService);
  favoritosService = inject(FavoritosService);
  private accesoUsuarioService = inject(AccesoUsuarioService);
  private router = inject(Router);

  sesion = this.accesoUsuarioService.sesion;

  categorias = signal<CategoriaConSubs[]>([]);
  menuAbierto = signal(false);
  categoriaActivaId = signal<number | null>(null);

  ngOnInit(): void {
    this.cargarMenu();
  }

  buscar(termino: string): void {
    const valor = termino.trim();
    if (!valor) return;
    this.router.navigate(['/buscar'], { queryParams: { q: valor } });
  }

  cerrarSesion(): void {
    this.accesoUsuarioService.cerrarSesion();
    this.router.navigate(['/']);
  }

  private cargarMenu(): void {
    this.categoriaService.listar().subscribe({
      next: respCat => {
        const categorias: ICategoria[] = respCat.data ?? [];

        this.subcategoriaService.listar().subscribe({
          next: respSub => {
            const subcategorias: ISubcategoria[] = respSub.data ?? [];

            this.categorias.set(
              categorias
                .filter(c => c.activo)
                .map(c => ({
                  ...c,
                  subcategorias: subcategorias.filter(
                    s => s.categoriaId === c.categoriaId
                  )
                }))
            );
          },
          error: err => {
            console.error('Error al cargar subcategorías del menú:', err);
            this.categorias.set(categorias.filter(c => c.activo).map(c => ({ ...c, subcategorias: [] })));
          }
        });
      },
      error: err => {
        console.error('Error al cargar categorías del menú:', err);
        this.categorias.set([]);
      }
    });
  }
}
