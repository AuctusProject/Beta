import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PerfomanceOpenPositionsComponent } from './perfomance-open-positions.component';

describe('PerfomanceOpenPositionsComponent', () => {
  let component: PerfomanceOpenPositionsComponent;
  let fixture: ComponentFixture<PerfomanceOpenPositionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PerfomanceOpenPositionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PerfomanceOpenPositionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
