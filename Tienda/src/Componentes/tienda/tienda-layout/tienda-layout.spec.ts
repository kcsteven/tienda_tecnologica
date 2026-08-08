import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TiendaLayoutComponent } from './tienda-layout';

describe('TiendaLayout', () => {
  // Instancia del componente
  let component: TiendaLayoutComponent;
  // Fixture para manejar el componente en las pruebas
  let fixture: ComponentFixture<TiendaLayoutComponent>;

  beforeEach(async () => {
    // Configura el módulo de pruebas importando el componente
    await TestBed.configureTestingModule({
      imports: [TiendaLayoutComponent],
    }).compileComponents();

    // Crea el componente de prueba
    fixture = TestBed.createComponent(TiendaLayoutComponent);
    component = fixture.componentInstance;
    // Espera a que el componente esté estable
    await fixture.whenStable();
  });

  it('should create', () => {
    // Verifica que el componente se haya creado correctamente
    expect(component).toBeTruthy();
  });
});
