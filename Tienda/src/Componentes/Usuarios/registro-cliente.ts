import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators
} from '@angular/forms';
import { IRegistroCliente } from '../../app/model/Usuarios/registro-cliente';
import { ITipoDocumento } from '../../app/model/Usuarios/tipo-documento';
import { RegistroClienteService } from '../../app/services/Usuarios/registro-cliente.service';

@Component({
  selector: 'app-registro-cliente',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './registro-cliente.html',
  styleUrl: './registro-cliente.scss'
})
export class RegistroClienteComponent implements OnInit {
  private fb = inject(FormBuilder);
  private registroClienteService = inject(RegistroClienteService);
  private changeDetectorRef = inject(ChangeDetectorRef);

  tiposDocumento: ITipoDocumento[] = [];
  cargandoTipos = true;
  enviando = false;
  enviado = false;
  mensajeExito = '';
  mensajeError = '';

  private contrasenasCoinciden: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const contrasena = control.get('contrasena')?.value;
    const confirmacion = control.get('confirmacionContrasena')?.value;
    return contrasena === confirmacion ? null : { contrasenasNoCoinciden: true };
  };

  formulario = this.fb.group(
    {
      tipoDocumentoId: [null as number | null, Validators.required],
      numeroDocumento: ['', [Validators.required, Validators.maxLength(30)]],
      nombre: ['', [Validators.required, Validators.maxLength(100)]],
      apellido: ['', [Validators.required, Validators.maxLength(100)]],
      fechaNacimiento: [''],
      telefono: ['', Validators.maxLength(20)],
      email: ['', [Validators.email, Validators.maxLength(100)]],
      nombreUsuario: ['', [Validators.required, Validators.maxLength(50)]],
      contrasena: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(128)]],
      confirmacionContrasena: ['', Validators.required],
      provincia: ['', [Validators.required, Validators.maxLength(100)]],
      canton: ['', [Validators.required, Validators.maxLength(100)]],
      distrito: ['', [Validators.required, Validators.maxLength(100)]],
      senaExacta: ['', Validators.maxLength(255)]
    },
    { validators: this.contrasenasCoinciden }
  );

  ngOnInit(): void {
    this.cargarTiposDocumento();
    this.formulario.controls.tipoDocumentoId.valueChanges.subscribe(() => {
      this.formulario.controls.numeroDocumento.updateValueAndValidity();
    });
  }

  registrar(): void {
    this.enviado = true;
    this.mensajeExito = '';
    this.mensajeError = '';

    if (this.formulario.invalid || this.enviando) {
      this.formulario.markAllAsTouched();
      return;
    }

    const valor = this.formulario.getRawValue();
    const registro: IRegistroCliente = {
      tipoDocumentoId: Number(valor.tipoDocumentoId),
      numeroDocumento: valor.numeroDocumento ?? '',
      nombre: valor.nombre ?? '',
      apellido: valor.apellido ?? '',
      fechaNacimiento: valor.fechaNacimiento || null,
      telefono: valor.telefono || null,
      email: valor.email || null,
      nombreUsuario: valor.nombreUsuario ?? '',
      contrasena: valor.contrasena ?? '',
      provincia: valor.provincia ?? '',
      canton: valor.canton ?? '',
      distrito: valor.distrito ?? '',
      senaExacta: valor.senaExacta || null
    };

    this.enviando = true;
    this.registroClienteService.registrar(registro).subscribe({
      next: respuesta => {
        this.enviando = false;
        if (respuesta.data) {
          this.mensajeExito = 'Tu registro fue completado correctamente.';
          this.formulario.reset();
          this.enviado = false;
          this.changeDetectorRef.markForCheck();
          return;
        }

        this.mensajeError = respuesta.error || 'No fue posible completar el registro.';
        this.changeDetectorRef.markForCheck();
      },
      error: error => {
        this.enviando = false;
        this.mensajeError = error?.error?.error || 'No fue posible completar el registro.';
        this.changeDetectorRef.markForCheck();
      }
    });
  }

  campoInvalido(nombre: string): boolean {
    const campo = this.formulario.get(nombre);
    return !!campo && campo.invalid && (campo.touched || this.enviado);
  }

  mensajeCampo(nombre: string): string {
    const campo = this.formulario.get(nombre);
    if (!campo?.errors) {
      return '';
    }

    if (campo.errors['required']) {
      return 'Este campo es obligatorio.';
    }
    if (campo.errors['email']) {
      return 'Ingresa un correo electrónico válido.';
    }
    if (campo.errors['minlength']) {
      return `Debe tener al menos ${campo.errors['minlength'].requiredLength} caracteres.`;
    }
    if (campo.errors['maxlength']) {
      return `No puede superar ${campo.errors['maxlength'].requiredLength} caracteres.`;
    }
    if (campo.errors['documento']) {
      return campo.errors['documento'];
    }
    if (campo.errors['contrasenasNoCoinciden']) {
      return 'Las contraseñas no coinciden.';
    }

    return 'Verifica el valor ingresado.';
  }

  contrasenasNoCoinciden(): boolean {
    return this.formulario.hasError('contrasenasNoCoinciden') &&
      (this.formulario.controls.confirmacionContrasena.touched || this.enviado);
  }

  private cargarTiposDocumento(): void {
    this.registroClienteService.listarTiposDocumento().subscribe({
      next: respuesta => {
        this.tiposDocumento = respuesta.data ?? [];
        this.cargandoTipos = false;
        if (this.tiposDocumento.length === 0) {
          this.mensajeError = 'No hay tipos de documento disponibles para el registro.';
        }
        this.changeDetectorRef.markForCheck();
      },
      error: error => {
        this.cargandoTipos = false;
        this.mensajeError = error?.error?.error || 'No fue posible cargar los tipos de documento.';
        this.changeDetectorRef.markForCheck();
      }
    });
  }

  private validadorDocumento(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const numeroDocumento = String(control.value ?? '').trim();
      const tipoDocumentoId = this.formulario?.controls.tipoDocumentoId.value;
      const tipoDocumento = this.tiposDocumento.find(x => x.tipoDocumentoId === tipoDocumentoId)?.nombre.toUpperCase();
      const numeroSinSeparadores = numeroDocumento.replace(/[\s-]/g, '');

      if (!numeroDocumento || !tipoDocumento) {
        return null;
      }

      if ((tipoDocumento === 'CÉDULA DE IDENTIDAD COSTARRICENSE' || tipoDocumento === 'CEDULA DE IDENTIDAD COSTARRICENSE') && !/^\d{9}$/.test(numeroSinSeparadores)) {
        return { documento: 'La cédula debe contener exactamente 9 dígitos.' };
      }

      if (tipoDocumento === 'DIMEX' && !/^\d{12}$/.test(numeroSinSeparadores)) {
        return { documento: 'El DIMEX debe contener exactamente 12 dígitos.' };
      }

      if (tipoDocumento === 'PASAPORTE' && numeroDocumento.length > 30) {
        return { documento: 'El pasaporte no puede superar 30 caracteres.' };
      }

      return null;
    };
  }

  constructor() {
    this.formulario.controls.numeroDocumento.addValidators(this.validadorDocumento());
  }
}
