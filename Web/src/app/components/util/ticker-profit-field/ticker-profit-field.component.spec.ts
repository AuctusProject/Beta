import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TickerProfitFieldComponent } from './ticker-profit-field.component';

describe('TickerProfitFieldComponent', () => {
  let component: TickerProfitFieldComponent;
  let fixture: ComponentFixture<TickerProfitFieldComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TickerProfitFieldComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TickerProfitFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
