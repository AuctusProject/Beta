import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EntryOptionComponent } from './entry-option.component';

describe('EntryOptionComponent', () => {
  let component: EntryOptionComponent;
  let fixture: ComponentFixture<EntryOptionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EntryOptionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EntryOptionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
