import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpertMiniCardComponent } from './expert-mini-card.component';

describe('ExpertMiniCardComponent', () => {
  let component: ExpertMiniCardComponent;
  let fixture: ComponentFixture<ExpertMiniCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpertMiniCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpertMiniCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
