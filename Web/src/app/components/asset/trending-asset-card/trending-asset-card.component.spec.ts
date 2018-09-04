import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TrendingAssetCardComponent } from './trending-asset-card.component';

describe('TrendingAssetCardComponent', () => {
  let component: TrendingAssetCardComponent;
  let fixture: ComponentFixture<TrendingAssetCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TrendingAssetCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TrendingAssetCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
