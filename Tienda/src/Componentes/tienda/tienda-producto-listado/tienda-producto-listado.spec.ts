import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TiendaProductoListado } from './tienda-producto-listado';

describe('TiendaProductoListado', () => {
  let component: TiendaProductoListado;
  let fixture: ComponentFixture<TiendaProductoListado>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TiendaProductoListado],
    }).compileComponents();

    fixture = TestBed.createComponent(TiendaProductoListado);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
