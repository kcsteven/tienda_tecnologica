import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TiendaProductoDetalle } from './tienda-producto-detalle';

describe('TiendaProductoDetalle', () => {
  let component: TiendaProductoDetalle;
  let fixture: ComponentFixture<TiendaProductoDetalle>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TiendaProductoDetalle],
    }).compileComponents();

    fixture = TestBed.createComponent(TiendaProductoDetalle);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
