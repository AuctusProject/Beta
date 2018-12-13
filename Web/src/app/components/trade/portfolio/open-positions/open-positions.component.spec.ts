import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OpenPositionsComponent } from './open-positions.component';

describe('OpenPositionsComponent', () => {
  let component: OpenPositionsComponent;
  let fixture: ComponentFixture<OpenPositionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OpenPositionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OpenPositionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
