import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GeneralRecommendationTagComponent } from './general-recommendation-tag.component';

describe('GeneralRecommendationTagComponent', () => {
  let component: GeneralRecommendationTagComponent;
  let fixture: ComponentFixture<GeneralRecommendationTagComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GeneralRecommendationTagComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GeneralRecommendationTagComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
