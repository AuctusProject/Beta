import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TopExpertsComponent } from './top-experts.component';

describe('TopExpertsComponent', () => {
  let component: TopExpertsComponent;
  let fixture: ComponentFixture<TopExpertsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TopExpertsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TopExpertsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
