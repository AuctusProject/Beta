import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HotsiteBlogSectionComponent } from './hotsite-blog-section.component';

describe('HotsiteBlogSectionComponent', () => {
  let component: HotsiteBlogSectionComponent;
  let fixture: ComponentFixture<HotsiteBlogSectionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HotsiteBlogSectionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HotsiteBlogSectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
