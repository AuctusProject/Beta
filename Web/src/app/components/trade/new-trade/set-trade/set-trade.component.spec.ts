import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SetTradeComponent } from './set-trade.component';

describe('SetTradeComponent', () => {
  let component: SetTradeComponent;
  let fixture: ComponentFixture<SetTradeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SetTradeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SetTradeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
