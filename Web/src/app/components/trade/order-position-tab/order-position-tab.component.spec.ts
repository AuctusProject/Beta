import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrderPositionTabComponent } from './order-position-tab.component';

describe('OrderPositionTabComponent', () => {
  let component: OrderPositionTabComponent;
  let fixture: ComponentFixture<OrderPositionTabComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrderPositionTabComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrderPositionTabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
