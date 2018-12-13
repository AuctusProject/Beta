import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TradeSummaryComponent } from './trade-summary.component';

describe('TradeSummaryComponent', () => {
  let component: TradeSummaryComponent;
  let fixture: ComponentFixture<TradeSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TradeSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TradeSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
