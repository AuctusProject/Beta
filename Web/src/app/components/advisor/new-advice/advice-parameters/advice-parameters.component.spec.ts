import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdviceParametersComponent } from './advice-parameters.component';

describe('AdviceParametersComponent', () => {
  let component: AdviceParametersComponent;
  let fixture: ComponentFixture<AdviceParametersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdviceParametersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdviceParametersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
