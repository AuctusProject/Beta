import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListAdvisorsComponent } from './list-advisors.component';

describe('ListAdvisorsComponent', () => {
  let component: ListAdvisorsComponent;
  let fixture: ComponentFixture<ListAdvisorsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListAdvisorsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListAdvisorsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
