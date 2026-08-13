import { CommonModule } from '@angular/common';
import {ChangeDetectorRef,
        Component,
       inject,
       OnInit
} from '@angular/core';
import { IClienteAdministracion } from '../../app/model/Usuarios/cliente-administracion';
import { ClienteAdministracionService } from '../../app/services/Usuarios/cliente-administracion.service';

@Component({
  selector: 'app-cliente',
  imports: [CommonModule],
  templateUrl: './cliente.html',
  styleUrl: './cliente.scss',
})


export class ClienteComponent implements OnInit {


  private clienteService = inject(ClienteAdministracionService);
  private changeDetectorRef = inject(ChangeDetectorRef);

  clientes: IClienteAdministracion[] = [];
  cargando = true;
  clienteProcesandoId: number | null = null;
  mensajeExito = '';
  mensajeError = '';

  //Carga todos los clientes al abrir
  ngOnInit(): void {
    this.cargarClientes();
  }

  //Confirma y solicita la activación/desactivación del cliente
  cambiarEstado(cliente: IClienteAdministracion): void {
    if (this.clienteProcesandoId !== null) {
      return;
    }

    const nuevoEstado = !cliente.activo;
    const accion = nuevoEstado ? 'activar' : 'desactivar';

    const confirmado = window.confirm(
      `¿Desea ${accion} al cliente ${cliente.nombreCompleto}?`
    );

    if (!confirmado) {
      return;
    }

    this.mensajeExito = '';
    this.mensajeError = '';
    this.clienteProcesandoId = cliente.clienteId;

    this.clienteService.cambiarEstado({

      clienteId: cliente.clienteId,
      activo: nuevoEstado}).subscribe({


      next: respuesta => {

        this.clienteProcesandoId = null;

        if (respuesta.error || respuesta.data !== true) {
          this.mensajeError =
            respuesta.error || 'No fue posible cambiar el estado del cliente.';

          this.changeDetectorRef.markForCheck();
          return;
        }

        //Actualiza únicamente la fila modificada sin volver a consultar la API
        this.clientes = this.clientes.map(item =>
          item.clienteId === cliente.clienteId
            ? { ...item, activo: nuevoEstado }
            : item
        );

        this.mensajeExito = nuevoEstado
          ? 'El cliente fue activado correctamente.'
          : 'El cliente fue desactivado correctamente.';

        this.changeDetectorRef.markForCheck();
      },
      error: error => {
        this.clienteProcesandoId = null;
        this.mensajeError =
          error?.error?.error ||
          'No fue posible cambiar el estado del cliente.';

        this.changeDetectorRef.markForCheck();
      }
    });
  }

  // Indica si el cliente está esperando una respuesta del servidor.
  estaProcesando(clienteId: number): boolean {
    return this.clienteProcesandoId === clienteId;
  }

  // Obtiene una sola vez la lista completa de clientes.
  private cargarClientes(): void {
    this.cargando = true;
    this.mensajeError = '';

    this.clienteService.listar().subscribe({

      next: respuesta => {
        this.cargando = false;

        if (respuesta.error) {

          this.clientes = [];
          this.mensajeError = respuesta.error;
          this.changeDetectorRef.markForCheck();
          return;
        }

        this.clientes = respuesta.data ?? [];
        this.changeDetectorRef.markForCheck();

      },
      error: error => {
        this.cargando = false;
        this.clientes = [];
        this.mensajeError =
          error?.error?.error || 'No fue posible cargar los clientes.';

        this.changeDetectorRef.markForCheck();

      }

    });

  }

}
