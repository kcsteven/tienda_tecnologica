import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DefauldHeader } from './defauld-header';

describe('DefauldHeader', () => {
  let component: DefauldHeader;
  let fixture: ComponentFixture<DefauldHeader>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DefauldHeader],
    }).compileComponents();

    fixture = TestBed.createComponent(DefauldHeader);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
