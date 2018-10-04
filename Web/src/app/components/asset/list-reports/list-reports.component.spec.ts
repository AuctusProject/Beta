import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListReportsComponent } from './list-reports.component';

describe('ListReportsComponent', () => {
  let component: ListReportsComponent;
  let fixture: ComponentFixture<ListReportsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListReportsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListReportsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
