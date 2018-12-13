import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PerformanceClosedPositionsComponent } from './perfomance-closed-positions.component';

describe('PerformanceClosedPositionsComponent', () => {
  let component: PerformanceClosedPositionsComponent;
  let fixture: ComponentFixture<PerformanceClosedPositionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PerformanceClosedPositionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PerformanceClosedPositionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
