import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AdvisorsRequestsComponent } from './advisors-requests.component';

describe('AdvisorsRequestsComponent', () => {
  let component: AdvisorsRequestsComponent;
  let fixture: ComponentFixture<AdvisorsRequestsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AdvisorsRequestsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AdvisorsRequestsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
