import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RecommendationDistributionComponent } from './recommendation-distribution.component';

describe('RecommendationDistributionComponent', () => {
  let component: RecommendationDistributionComponent;
  let fixture: ComponentFixture<RecommendationDistributionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RecommendationDistributionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RecommendationDistributionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
