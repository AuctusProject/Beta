import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TradingContestComponent } from './trading-contest.component';

describe('TradingContestComponent', () => {
  let component: TradingContestComponent;
  let fixture: ComponentFixture<TradingContestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TradingContestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TradingContestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
