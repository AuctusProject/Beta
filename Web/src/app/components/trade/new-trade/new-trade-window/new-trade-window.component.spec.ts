import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewTradeWindowComponent } from './new-trade-window.component';

describe('NewTradeWindowComponent', () => {
  let component: NewTradeWindowComponent;
  let fixture: ComponentFixture<NewTradeWindowComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewTradeWindowComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewTradeWindowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
