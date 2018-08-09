import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BecomeAdvisorComponent } from './become-advisor.component';

describe('BecomeAdvisorComponent', () => {
  let component: BecomeAdvisorComponent;
  let fixture: ComponentFixture<BecomeAdvisorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BecomeAdvisorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BecomeAdvisorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
