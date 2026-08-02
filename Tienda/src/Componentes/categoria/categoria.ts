import { Component, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ColComponent, RowComponent } from '@coreui/angular';
import { FormUtils } from '../../app/Utilitarios/Utils';
import { ICategoria } from '../../app/model/ICategoria';

@Component({
  selector: 'app-categoria',
  standalone: true,
  imports: [
    ColComponent,
    RowComponent,
    ReactiveFormsModule
  ],
  templateUrl: './categoria.html',
  styleUrl: './categoria.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class CategoriaComponent {

  formUtils = FormUtils;
  private FB = inject(FormBuilder);

  // 🔹 Datos de ejemplo (mock) solo para diseñar. Luego los reemplazamos por el servicio real.
  categorias: ICategoria[] = [
    { categoriaId: 1, nombre: 'Laptops', descripcion: 'Computadoras portátiles', activo: true, creadoEn: new Date(), creadoPor: 'admin' },
    { categoriaId: 2, nombre: 'Celulares', descripcion: 'Teléfonos inteligentes', activo: true, creadoEn: new Date(), creadoPor: 'admin' },
    { categoriaId: 3, nombre: 'Accesorios', descripcion: 'Fundas, cargadores, etc.', activo: false, creadoEn: new Date(), creadoPor: 'admin' },
  ];

  myFormulario: FormGroup = this.FB.group({
    categoriaId: [0, [Validators.required]],
    nombreCategoria: ['', [Validators.required, Validators.maxLength(150)]],
    activo: [false]
  });

  // Al hacer clic en "Editar" en una fila, carga esos datos en el formulario de arriba
  onEditar(categoria: ICategoria) {
    this.myFormulario.patchValue({
      categoriaId: categoria.categoriaId,
      nombreCategoria: categoria.nombre,
      activo: categoria.activo
    });
  }

  onNuevo() {
    this.myFormulario.reset({ categoriaId: 0, nombreCategoria: '', activo: false });
  }

  onEliminar(categoria: ICategoria) {
    // Por ahora solo lo saca de la lista mock; luego será una llamada al servicio
    this.categorias = this.categorias.filter(c => c.categoriaId !== categoria.categoriaId);
  }

  onGuardar() {

  }
}
