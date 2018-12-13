import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssetMiniViewTableComponent } from './asset-mini-view-table.component';

describe('AssetMiniViewTableComponent', () => {
  let component: AssetMiniViewTableComponent;
  let fixture: ComponentFixture<AssetMiniViewTableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssetMiniViewTableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssetMiniViewTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
