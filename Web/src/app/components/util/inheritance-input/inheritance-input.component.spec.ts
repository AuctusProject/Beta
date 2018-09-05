import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InheritanceInputComponent } from './inheritance-input.component';

describe('InheritanceInputComponent', () => {
  let component: InheritanceInputComponent;
  let fixture: ComponentFixture<InheritanceInputComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InheritanceInputComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InheritanceInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
