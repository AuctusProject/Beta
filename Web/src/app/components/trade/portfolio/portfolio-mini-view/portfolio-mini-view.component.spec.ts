import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PortfolioMiniViewComponent } from './portfolio-mini-view.component';

describe('PortfolioMiniViewComponent', () => {
  let component: PortfolioMiniViewComponent;
  let fixture: ComponentFixture<PortfolioMiniViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PortfolioMiniViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PortfolioMiniViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
