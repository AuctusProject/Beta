import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewAdviceComponent } from './new-advice.component';

describe('NewAdviceComponent', () => {
  let component: NewAdviceComponent;
  let fixture: ComponentFixture<NewAdviceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewAdviceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewAdviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
