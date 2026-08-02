import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TiendaLayout } from './tienda-layout';

describe('TiendaLayout', () => {
  let component: TiendaLayout;
  let fixture: ComponentFixture<TiendaLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TiendaLayout],
    }).compileComponents();

    fixture = TestBed.createComponent(TiendaLayout);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
