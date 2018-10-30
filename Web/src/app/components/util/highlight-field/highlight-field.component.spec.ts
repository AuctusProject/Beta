import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HighlightFieldComponent } from './highlight-field.component';

describe('HighlightFieldComponent', () => {
  let component: HighlightFieldComponent;
  let fixture: ComponentFixture<HighlightFieldComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HighlightFieldComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HighlightFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
