import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MessageFullscreenModalComponent } from './message-fullscreen-modal.component';

describe('MessageFullscreenModalComponent', () => {
  let component: MessageFullscreenModalComponent;
  let fixture: ComponentFixture<MessageFullscreenModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MessageFullscreenModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MessageFullscreenModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
