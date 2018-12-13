import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MiniTradingViewChartComponent } from './mini-trading-view-chart.component';

describe('MiniTradingViewChartComponent', () => {
  let component: MiniTradingViewChartComponent;
  let fixture: ComponentFixture<MiniTradingViewChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MiniTradingViewChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MiniTradingViewChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
