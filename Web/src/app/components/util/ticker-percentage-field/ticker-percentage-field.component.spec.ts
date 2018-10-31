import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TickerPercentageFieldComponent } from './ticker-percentage-field.component';

describe('TickerPercentageFieldComponent', () => {
  let component: TickerPercentageFieldComponent;
  let fixture: ComponentFixture<TickerPercentageFieldComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TickerPercentageFieldComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TickerPercentageFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
