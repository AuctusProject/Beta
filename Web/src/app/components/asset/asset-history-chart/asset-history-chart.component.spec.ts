import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssetHistoryChartComponent } from './asset-history-chart.component';

describe('AssetHistoryChartComponent', () => {
  let component: AssetHistoryChartComponent;
  let fixture: ComponentFixture<AssetHistoryChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssetHistoryChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssetHistoryChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
