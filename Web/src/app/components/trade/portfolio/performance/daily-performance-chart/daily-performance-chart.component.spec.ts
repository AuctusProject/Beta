import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DailyPerformanceChartComponent } from './daily-performance-chart.component';

describe('DailyPerformanceChartComponent', () => {
  let component: DailyPerformanceChartComponent;
  let fixture: ComponentFixture<DailyPerformanceChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DailyPerformanceChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DailyPerformanceChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
