import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TradeSignalComponent } from './trade-signal.component';

describe('TradeSignalComponent', () => {
  let component: TradeSignalComponent;
  let fixture: ComponentFixture<TradeSignalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TradeSignalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TradeSignalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
