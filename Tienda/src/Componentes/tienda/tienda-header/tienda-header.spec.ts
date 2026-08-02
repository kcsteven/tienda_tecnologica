import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TiendaHeader } from './tienda-header';

describe('TiendaHeader', () => {
  let component: TiendaHeader;
  let fixture: ComponentFixture<TiendaHeader>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TiendaHeader],
    }).compileComponents();

    fixture = TestBed.createComponent(TiendaHeader);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
