import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HotsiteHeaderComponent } from './hotsite-header.component';

describe('HotsiteHeaderComponent', () => {
  let component: HotsiteHeaderComponent;
  let fixture: ComponentFixture<HotsiteHeaderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HotsiteHeaderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HotsiteHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
