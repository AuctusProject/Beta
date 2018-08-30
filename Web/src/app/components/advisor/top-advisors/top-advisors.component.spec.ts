import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TopAdvisorsComponent } from './top-advisors.component';

describe('TopAdvisorsComponent', () => {
  let component: TopAdvisorsComponent;
  let fixture: ComponentFixture<TopAdvisorsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TopAdvisorsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TopAdvisorsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
