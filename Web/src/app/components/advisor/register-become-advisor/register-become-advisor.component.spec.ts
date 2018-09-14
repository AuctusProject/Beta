import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RegisterBecomeAdvisorComponent } from './register-become-advisor.component';

describe('RegisterBecomeAdvisorComponent', () => {
  let component: RegisterBecomeAdvisorComponent;
  let fixture: ComponentFixture<RegisterBecomeAdvisorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RegisterBecomeAdvisorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RegisterBecomeAdvisorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
