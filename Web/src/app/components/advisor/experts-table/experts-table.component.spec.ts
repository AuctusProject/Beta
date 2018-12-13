import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpertsTableComponent } from './experts-table.component';

describe('ExpertsTableComponent', () => {
  let component: ExpertsTableComponent;
  let fixture: ComponentFixture<ExpertsTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpertsTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpertsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
