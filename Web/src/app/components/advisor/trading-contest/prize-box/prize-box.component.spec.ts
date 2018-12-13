import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PrizeBoxComponent } from './prize-box.component';

describe('PrizeBoxComponent', () => {
  let component: PrizeBoxComponent;
  let fixture: ComponentFixture<PrizeBoxComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PrizeBoxComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PrizeBoxComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
