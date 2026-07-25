import { Component, CUSTOM_ELEMENTS_SCHEMA, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ColComponent, RowComponent } from '@coreui/angular';
import { FormUtils } from '../../app/Utilitarios/Utils';

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

  myFormulario: FormGroup = this.FB.group({
    categoriaId: [0, [Validators.required]],
    NombreCategoria: [0, [Validators.required, Validators.maxLength(150)]],
    activo: [false]
  });

  onGuardar() {

  }
}
