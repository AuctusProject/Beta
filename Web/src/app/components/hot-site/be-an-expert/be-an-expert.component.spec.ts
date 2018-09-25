import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BeAnExpertComponent } from './be-an-expert.component';

describe('BeAnExpertComponent', () => {
  let component: BeAnExpertComponent;
  let fixture: ComponentFixture<BeAnExpertComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BeAnExpertComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BeAnExpertComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
