import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HotSiteComponent } from './hot-site.component';

describe('HotSiteComponent', () => {
  let component: HotSiteComponent;
  let fixture: ComponentFixture<HotSiteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HotSiteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HotSiteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
