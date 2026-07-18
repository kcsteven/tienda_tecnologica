import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DefauldFooter } from './defauld-footer';

describe('DefauldFooter', () => {
  let component: DefauldFooter;
  let fixture: ComponentFixture<DefauldFooter>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DefauldFooter],
    }).compileComponents();

    fixture = TestBed.createComponent(DefauldFooter);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
