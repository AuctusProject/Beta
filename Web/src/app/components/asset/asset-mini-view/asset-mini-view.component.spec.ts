import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssetMiniViewComponent } from './asset-mini-view.component';

describe('AssetMiniViewComponent', () => {
  let component: AssetMiniViewComponent;
  let fixture: ComponentFixture<AssetMiniViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssetMiniViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssetMiniViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
