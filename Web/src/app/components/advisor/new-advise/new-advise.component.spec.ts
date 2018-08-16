import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NewAdviseComponent } from './new-advise.component';

describe('NewAdviseComponent', () => {
  let component: NewAdviseComponent;
  let fixture: ComponentFixture<NewAdviseComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewAdviseComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewAdviseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
