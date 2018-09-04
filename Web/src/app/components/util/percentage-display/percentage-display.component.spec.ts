import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PercentageDisplayComponent } from './percentage-display.component';

describe('PercentageDisplayComponent', () => {
  let component: PercentageDisplayComponent;
  let fixture: ComponentFixture<PercentageDisplayComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PercentageDisplayComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PercentageDisplayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
