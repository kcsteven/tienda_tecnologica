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



  // Catalogo de provincias validas
  readonly provincias = ['San José', 'Alajuela', 'Cartago', 'Heredia', 'Guanacaste', 'Puntarenas', 'Limón'];

  //Catalogo de cantones validos
  readonly cantonesPorProvincia: Record<string, string[]> = {
    'San José': ['San José', 'Escazú', 'Desamparados', 'Puriscal', 'Tarrazú', 'Aserrí', 'Mora', 'Goicoechea', 'Santa Ana', 'Alajuelita', 'Vázquez de Coronado', 'Acosta', 'Tibás', 'Moravia', 'Montes de Oca', 'Turrubares', 'Dota', 'Curridabat', 'Pérez Zeledón', 'León Cortés Castro'],

    Alajuela: ['Alajuela', 'San Ramón', 'Grecia', 'San Mateo', 'Atenas', 'Naranjo', 'Palmares', 'Poás', 'Orotina', 'San Carlos', 'Zarcero', 'Sarchí', 'Upala', 'Los Chiles', 'Guatuso', 'Río Cuarto'],

    Cartago: ['Cartago', 'Paraíso', 'La Unión', 'Jiménez', 'Turrialba', 'Alvarado', 'Oreamuno', 'El Guarco'],

    Heredia: ['Heredia', 'Barva', 'Santo Domingo', 'Santa Bárbara', 'San Rafael', 'San Isidro', 'Belén', 'Flores', 'San Pablo', 'Sarapiquí'],

    Guanacaste: ['Liberia', 'Nicoya', 'Santa Cruz', 'Bagaces', 'Carrillo', 'Cañas', 'Abangares', 'Tilarán', 'Nandayure', 'La Cruz', 'Hojancha'],

    Puntarenas: ['Puntarenas', 'Esparza', 'Buenos Aires', 'Montes de Oro', 'Osa', 'Quepos', 'Golfito', 'Coto Brus', 'Parrita', 'Corredores', 'Garabito', 'Monteverde', 'Puerto Jiménez'],

    Limón: ['Limón', 'Pococí', 'Siquirres', 'Talamanca', 'Matina', 'Guácimo']
  };


  readonly fechaMaximaNacimiento = this.obtenerFechaMaximaNacimiento();

  //Verifica que las contraseñas coincidan
  private contrasenasCoinciden: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {

    const contrasena = control.get('contrasena')?.value;
    const confirmacion = control.get('confirmacionContrasena')?.value;
    return contrasena === confirmacion ? null : { contrasenasNoCoinciden: true };
  };

  formulario = this.fb.group(
    {

      tipoDocumentoId: [null as number | null, Validators.required],
      numeroDocumento: ['', [Validators.required, Validators.maxLength(30), this.validadorDocumento()]],
      nombre: ['', [Validators.required, Validators.maxLength(100), this.validadorSoloLetras()]],
      apellido: ['', [Validators.required, Validators.maxLength(100), this.validadorSoloLetras()]],
      fechaNacimiento: ['', [Validators.required, this.validadorMayorEdad()]],
      telefono: ['', Validators.maxLength(20)],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(100), this.validadorCorreoGmail()]],
      nombreUsuario: ['', [Validators.required, Validators.maxLength(50)]],
      contrasena: ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.maxLength(128),
        Validators.pattern(/^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])[\s\S]+$/)
      ]],
      confirmacionContrasena: ['', Validators.required],
      provincia: ['', [Validators.required, this.validadorProvincia()]],
      canton: [{ value: '', disabled: true }, [Validators.required, this.validadorCanton()]],
      distrito: ['', [Validators.required, Validators.maxLength(100), this.validadorSoloLetras()]],
      senaExacta: ['', Validators.maxLength(255)]

    },
    { validators: this.contrasenasCoinciden }
  );


  //Carga los tipos de documentos y configura las validaciones
  ngOnInit(): void {
    this.cargarTiposDocumento();



    this.formulario.controls.tipoDocumentoId.valueChanges.subscribe(() => {

      this.actualizarValidacionesDocumento();
    });

   this.formulario.controls.provincia.valueChanges.subscribe(() => {

      this.actualizarCantones();
    });
  }


  //Envia los datos del formulario al Back-End
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
      numeroDocumento: this.normalizarNumeroDocumento(valor.numeroDocumento),
      nombre: this.normalizarTexto(valor.nombre),
      apellido: this.normalizarTexto(valor.apellido),
      fechaNacimiento: valor.fechaNacimiento || null,
      telefono: valor.telefono || null,
      email: this.normalizarTexto(valor.email).toLowerCase(),
      nombreUsuario: this.normalizarTexto(valor.nombreUsuario),
      contrasena: valor.contrasena ?? '',
      provincia: this.normalizarTexto(valor.provincia),
      canton: this.normalizarTexto(valor.canton),
      distrito: this.normalizarTexto(valor.distrito),
      senaExacta: this.normalizarTexto(valor.senaExacta) || null
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

  //Indica si un campo debe mostrar un error
  campoInvalido(nombre: string): boolean {

    const campo = this.formulario.get(nombre);
    return !!campo && campo.invalid && (campo.touched || campo.dirty || this.enviado);
  }


  //Muestra los errores en el campo/espacio correspondiente
  mensajeCampo(nombre: string): string {
    const campo = this.formulario.get(nombre);


    if (!campo?.errors) {
      return '';
    }

    if (campo.errors['required']) {
      return 'Este campo es obligatorio.';
    }


    if (campo.errors['soloLetras']) {

      if (nombre === 'nombre') {
        return 'El nombre solo puede contener letras';

      }
      if (nombre === 'apellido') {
        return 'El apellido solo puede contener letras';

      }
      return 'El distrito solo puede contener letras';

    }

    if (campo.errors['correoGmail'] || campo.errors['email']) {
      return 'Ingrese un correo valido';

    }
    if (campo.errors['documento']) {
      return campo.errors['documento'];

    }
    if (campo.errors['mayorEdad']) {
      return 'Necesitas ser mayor de 18 años';

    }
    if (campo.errors['provinciaInvalida']) {
      return 'Seleccione una provincia válida.';

    }
    if (campo.errors['cantonInvalido']) {
      return 'Seleccione un cantón válido para la provincia.';

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
    if (campo.errors['pattern']) {
      return 'Debe incluir al menos una mayúscula, una minúscula y un dígito.';

    }
    if (campo.errors['documento']) {
      return campo.errors['documento'];

    }
    if (campo.errors['contrasenasNoCoinciden']) {
     return 'Las contraseñas no coinciden.';

    }


    return 'Verifica el valor ingresado.';
  }

  //Avisa si las contraseñas no coinciden
  contrasenasNoCoinciden(): boolean {
    return this.formulario.hasError('contrasenasNoCoinciden') &&
      (this.formulario.controls.confirmacionContrasena.touched || this.enviado);
  }

  //carga los tipos de documentos
  private cargarTiposDocumento(): void {
    this.registroClienteService.listarTiposDocumento().subscribe({
      next: respuesta => {
        this.tiposDocumento = respuesta.data ?? [];
        this.actualizarValidacionesDocumento();
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
  //Valida los datos del documento
  private validadorDocumento(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const numeroDocumento = String(control.value ?? '');
      const tipoDocumento = this.tipoDocumentoSeleccionado();

      if (!numeroDocumento || !tipoDocumento) {
        return null;
      }

      if (this.esCedula(tipoDocumento)) {
        if (!/^\d+$/.test(numeroDocumento)) {
          return { documento: 'El numero de documento solo puede contener numeros' };
        }
        return /^\d{9}$/.test(numeroDocumento)
          ? null
          : { documento: 'La cédula debe contener exactamente 9 dígitos.' };
      }

      if (tipoDocumento === 'DIMEX') {
        if (!/^\d+$/.test(numeroDocumento)) {
          return { documento: 'El numero de documento solo puede contener numeros' };
        }
        return /^\d{11,12}$/.test(numeroDocumento)
          ? null
          : { documento: 'El DIMEX debe contener 11 o 12 dígitos.' };
      }

      if (tipoDocumento === 'PASAPORTE') {
        if (!/^[A-Za-z0-9]+$/.test(numeroDocumento)) {
          return { documento: 'El pasaporte solo puede contener letras y números.' };
        }
        return numeroDocumento.length >= 5 && numeroDocumento.length <= 20
          ? null
          : { documento: 'El pasaporte debe contener entre 5 y 20 caracteres.' };
      }

      return { documento: 'El tipo de documento indicado no está admitido para el registro.' };
    };
  }

  // cambia las validaciones según se selecciona Cédula, DIMEX o pasaporte
  private actualizarValidacionesDocumento(): void {
    const control = this.formulario.controls.numeroDocumento;
    control.setValidators([
      Validators.required,
      Validators.maxLength(this.longitudMaximaNumeroDocumento),
      this.validadorDocumento()
    ]);
    control.updateValueAndValidity();
  }

  //hace las letras se pongan en mayuscula cuando el documento es pasaporte
  normalizarPasaporte(): void {
    if (this.tipoDocumentoSeleccionado() !== 'PASAPORTE') {
      return;
    }

    const control = this.formulario.controls.numeroDocumento;
    const valorEnMayuscula = String(control.value ?? '').toUpperCase();
    if (valorEnMayuscula !== control.value) {
      control.setValue(valorEnMayuscula);
    }
  }

  //Agarra los cantones disponibles segun la provincia
  get cantonesDisponibles(): readonly string[] {
    return this.cantonesPorProvincia[this.formulario.controls.provincia.value ?? ''] ?? [];
  }


  //define la cantidad maxima segun el documento
  get longitudMaximaNumeroDocumento(): number {
    const tipoDocumento = this.tipoDocumentoSeleccionado();
    if (this.esCedula(tipoDocumento)) {
      return 9;
    }
    if (tipoDocumento === 'DIMEX') {
      return 12;
    }
    if (tipoDocumento === 'PASAPORTE') {
      return 20;
    }
    return 30;
  }

  //Cambia el tipo de entrada según el documento
  get modoEntradaNumeroDocumento(): 'numeric' | 'text' {
    const tipoDocumento = this.tipoDocumentoSeleccionado();
    return this.esCedula(tipoDocumento) || tipoDocumento === 'DIMEX' ? 'numeric' : 'text';
  }

  //proporciona ayuda segun el tipo de documento seleccionado
  get ayudaNumeroDocumento(): string {
    const tipoDocumento = this.tipoDocumentoSeleccionado();
    if (this.esCedula(tipoDocumento)) {
      return 'X-XXXX-XXXX';
    }
    if (tipoDocumento === 'DIMEX') {
      return '11 o 12 dígitos';
    }
    if (tipoDocumento === 'PASAPORTE') {
      return '5 a 20 letras o números';
    }
    return '';
  }

  //Actualiza los cantones cuando cambia de provincia
  private actualizarCantones(): void {
    const control = this.formulario.controls.canton;
    control.reset('', { emitEvent: false });

    if (this.cantonesDisponibles.length === 0) {
      control.disable({ emitEvent: false });
    } else {
      control.enable({ emitEvent: false });
    }

    control.updateValueAndValidity({ emitEvent: false });
  }


  //valida que solamente se puedan poner letras
  private validadorSoloLetras(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const valor = String(control.value ?? '');
      return !valor || /^(?=.*\p{L})[\p{L} ]+$/u.test(valor) ? null : { soloLetras: true };
    };
  }


  //Valida que el correo termine en @gmail.com
  private validadorCorreoGmail(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const correo = String(control.value ?? '');
      return !correo || /^[A-Za-z0-9._+-]+@gmail\.com$/i.test(correo) ? null : { correoGmail: true };
    };
  }

  // Valida que sea mayor de edad
  private validadorMayorEdad(): ValidatorFn {


    return (control: AbstractControl): ValidationErrors | null => {
      const valor = String(control.value ?? '');
      if (!valor) {
        return null;
      }

      const [anio, mes, dia] = valor.split('-').map(Number);
      const fechaNacimiento = new Date(anio, mes - 1, dia);
      const fechaValida = fechaNacimiento.getFullYear() === anio &&
        fechaNacimiento.getMonth() === mes - 1 && fechaNacimiento.getDate() === dia;

      return fechaValida && fechaNacimiento <= this.fechaDeMayoriaEdad() ? null : { mayorEdad: true };
    };
  }

  //Valida que la provincia sea valida
  private validadorProvincia(): ValidatorFn {

    return (control: AbstractControl): ValidationErrors | null => {
      const provincia = String(control.value ?? '');
      return !provincia || this.provincias.includes(provincia) ? null : { provinciaInvalida: true };
    };
  }
  //Valida que el canton sea valido
  private validadorCanton(): ValidatorFn {

    return (control: AbstractControl): ValidationErrors | null => {
      const canton = String(control.value ?? '');
      return !canton || this.cantonesDisponibles.includes(canton) ? null : { cantonInvalido: true };
    };
  }

  //Verifica que tipo de documento se selecciono
  private tipoDocumentoSeleccionado(): string {

    const tipoDocumentoId = this.formulario?.controls.tipoDocumentoId.value;
    const nombre = this.tiposDocumento.find(x => x.tipoDocumentoId === tipoDocumentoId)?.nombre ?? '';
    return nombre.normalize('NFD').replace(/[\u0300-\u036f]/g, '').trim().toUpperCase();
  }

  private esCedula(tipoDocumento: string): boolean {
    return tipoDocumento === 'CEDULA DE IDENTIDAD COSTARRICENSE';
  }

  private normalizarNumeroDocumento(valor: string | null | undefined): string {


    const numeroDocumento = String(valor ?? '');
    return this.tipoDocumentoSeleccionado() === 'PASAPORTE'
      ? numeroDocumento.trim().toUpperCase()
      : numeroDocumento.trim();
  }

  private normalizarTexto(valor: string | null | undefined): string {
    return String(valor ?? '').trim();
  }

  //Calcula que sea mayor de edad segun la fecha actual
  private fechaDeMayoriaEdad(): Date {


    const hoy = new Date();
    return new Date(hoy.getFullYear() - 18, hoy.getMonth(), hoy.getDate());
  }

  //Calcula la fecha maxima en la que pudo nacer el cliente
  private obtenerFechaMaximaNacimiento(): string {


    const fecha = this.fechaDeMayoriaEdad();
    const mes = String(fecha.getMonth() + 1).padStart(2, '0');
    const dia = String(fecha.getDate()).padStart(2, '0');
    return `${fecha.getFullYear()}-${mes}-${dia}`;
  }

  constructor() {
    this.actualizarValidacionesDocumento();
  }
}
