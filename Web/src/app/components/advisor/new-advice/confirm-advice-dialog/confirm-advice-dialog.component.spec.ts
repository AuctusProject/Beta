import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmAdviceDialogComponent } from './confirm-advice-dialog.component';

describe('ConfirmAdviceDialogComponent', () => {
  let component: ConfirmAdviceDialogComponent;
  let fixture: ComponentFixture<ConfirmAdviceDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfirmAdviceDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmAdviceDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
