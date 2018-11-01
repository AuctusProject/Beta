import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TickerFieldComponent } from './ticker-field.component';

describe('TickerFieldComponent', () => {
  let component: TickerFieldComponent;
  let fixture: ComponentFixture<TickerFieldComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TickerFieldComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TickerFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
