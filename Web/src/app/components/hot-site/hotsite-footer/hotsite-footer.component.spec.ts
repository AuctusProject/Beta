import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HotsiteFooterComponent } from './hotsite-footer.component';

describe('HotsiteFooterComponent', () => {
  let component: HotsiteFooterComponent;
  let fixture: ComponentFixture<HotsiteFooterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HotsiteFooterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HotsiteFooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
