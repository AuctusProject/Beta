import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditTradeValueComponent } from './edit-trade-value.component';

describe('EditTradeValueComponent', () => {
  let component: EditTradeValueComponent;
  let fixture: ComponentFixture<EditTradeValueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditTradeValueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditTradeValueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
