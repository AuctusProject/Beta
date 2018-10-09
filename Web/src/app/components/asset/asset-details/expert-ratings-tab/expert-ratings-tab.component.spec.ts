import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpertRatingsTabComponent } from './expert-ratings-tab.component';

describe('ExpertRatingsTabComponent', () => {
  let component: ExpertRatingsTabComponent;
  let fixture: ComponentFixture<ExpertRatingsTabComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpertRatingsTabComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpertRatingsTabComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
