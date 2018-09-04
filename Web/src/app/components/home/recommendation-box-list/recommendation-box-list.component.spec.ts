import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RecommendationBoxListComponent } from './recommendation-box-list.component';

describe('RecommendationBoxListComponent', () => {
  let component: RecommendationBoxListComponent;
  let fixture: ComponentFixture<RecommendationBoxListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RecommendationBoxListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RecommendationBoxListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
