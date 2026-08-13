import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { UltimaCompraService } from '../../../app/services/ultima-compra';

@Component({
  selector: 'app-tienda-pedido-confirmacion',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './tienda-pedido-confirmacion.html',
  styleUrl: './tienda-pedido-confirmacion.scss'
})
export class TiendaPedidoConfirmacionComponent {
  private route = inject(ActivatedRoute);
  private ultimaCompraService = inject(UltimaCompraService);

  pedidoId = Number(this.route.snapshot.paramMap.get('id'));
  resumen = this.ultimaCompraService.resumen();
}
