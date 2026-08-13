import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AccesoUsuarioService } from '../../app/services/Usuarios/acceso-usuario.service';

@Component({
  selector: 'app-inicio-sesion',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './inicio-sesion.html',
  styleUrl: './inicio-sesion.scss'
})
export class InicioSesionComponent {
  private fb = inject(FormBuilder);
  private accesoUsuarioService = inject(AccesoUsuarioService);
  private router = inject(Router);

  enviando = false;
  enviado = false;
  mensajeError = '';

  formulario = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
    contrasena: ['', [Validators.required, Validators.maxLength(128)]]
  });


  //valida las credenciales, inicia sesion y redirige segun el rol
  iniciarSesion(): void {
    this.enviado = true;
    this.mensajeError = '';

    if (this.formulario.invalid || this.enviando) {
      this.formulario.markAllAsTouched();
      return;
    }

    const { email, contrasena } = this.formulario.getRawValue();
    this.enviando = true;

    this.accesoUsuarioService.iniciarSesion({ email: email.trim(), contrasena }).subscribe({
      next: respuesta => {
        this.enviando = false;

        if (!respuesta.data) {
          this.mensajeError = 'Correo o contraseña incorrectos';
          return;
        }

        this.router.navigate([respuesta.data.rol === 'Empleado' ? '/admin' : '/']);
      },
      error: error => {
        this.enviando = false;
        this.mensajeError = error?.status === 401
          ? 'Correo o contraseña incorrectos'
          : 'No fue posible iniciar sesión';
      }
    });
  }

  //muestra si es campo es invalido
  campoInvalido(nombre: 'email' | 'contrasena'): boolean {
    const campo = this.formulario.controls[nombre];
    return campo.invalid && (campo.touched || this.enviado);
  }


  //Muestra el mensaje en el campo especifico
  mensajeCampo(nombre: 'email' | 'contrasena'): string {
    const campo = this.formulario.controls[nombre];
    if (campo.hasError('required')) {
      return nombre === 'email' ? 'El correo electrónico es obligatorio' : 'La contraseña es obligatoria';
    }

    return nombre === 'email' ? 'Ingrese un correo válido' : 'Verifica la contraseña ingresada';
  }
}
