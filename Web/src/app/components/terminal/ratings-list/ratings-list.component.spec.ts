import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RatingsListComponent } from './ratings-list.component';

describe('RatingsListComponent', () => {
  let component: RatingsListComponent;
  let fixture: ComponentFixture<RatingsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RatingsListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RatingsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
