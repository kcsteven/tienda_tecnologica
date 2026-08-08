import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TiendaHomeComponent } from './tienda-home';

describe('TiendaHomeComponent', () => {
  // Instancia del componente
  let component: TiendaHomeComponent;
  // Fixture para manejar el componente en las pruebas
  let fixture: ComponentFixture<TiendaHomeComponent>;

  beforeEach(async () => {
    // Configura el módulo de pruebas importando el componente
    await TestBed.configureTestingModule({
      imports: [TiendaHomeComponent],
    }).compileComponents();

    // Crea el componente de prueba
    fixture = TestBed.createComponent(TiendaHomeComponent);
    component = fixture.componentInstance;
    // Espera a que el componente esté estable
    await fixture.whenStable();
  });

  it('should create', () => {
    // Verifica que el componente se haya creado correctamente
    expect(component).toBeTruthy();
  });
});
