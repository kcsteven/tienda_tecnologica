import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TiendaProductoListadoComponent } from './tienda-producto-listado';

describe('TiendaProductoListado', () => {
  // Instancia del componente
  let component: TiendaProductoListadoComponent;
  // Fixture para manejar el componente en las pruebas
  let fixture: ComponentFixture<TiendaProductoListadoComponent>;

  beforeEach(async () => {
    // Configura el módulo de pruebas importando el componente
    await TestBed.configureTestingModule({
      imports: [TiendaProductoListadoComponent],
    }).compileComponents();

    // Crea el componente de prueba
    fixture = TestBed.createComponent(TiendaProductoListadoComponent);
    component = fixture.componentInstance;
    // Espera a que el componente esté estable
    await fixture.whenStable();
  });

  it('should create', () => {
    // Verifica que el componente se haya creado correctamente
    expect(component).toBeTruthy();
  });
});
