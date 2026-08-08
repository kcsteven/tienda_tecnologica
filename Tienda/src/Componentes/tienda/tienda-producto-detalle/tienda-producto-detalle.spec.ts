import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TiendaProductoDetalleComponent } from './tienda-producto-detalle';

describe('TiendaProductoDetalle', () => {
  // Instancia del componente
  let component: TiendaProductoDetalleComponent;
  // Fixture para manejar el componente en las pruebas
  let fixture: ComponentFixture<TiendaProductoDetalleComponent>;

  beforeEach(async () => {
    // Configura el módulo de pruebas importando el componente
    await TestBed.configureTestingModule({
      imports: [TiendaProductoDetalleComponent],
    }).compileComponents();

    // Crea el componente de prueba
    fixture = TestBed.createComponent(TiendaProductoDetalleComponent);
    component = fixture.componentInstance;
    // Espera a que el componente esté estable
    await fixture.whenStable();
  });

  it('should create', () => {
    // Verifica que el componente se haya creado correctamente
    expect(component).toBeTruthy();
  });
});
