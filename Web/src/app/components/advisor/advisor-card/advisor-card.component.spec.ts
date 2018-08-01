import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdvisorCardComponent } from './advisor-card.component';

describe('AdvisorCardComponent', () => {
  let component: AdvisorCardComponent;
  let fixture: ComponentFixture<AdvisorCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdvisorCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdvisorCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
