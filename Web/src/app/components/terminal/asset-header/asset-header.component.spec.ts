import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssetHeaderComponent } from './asset-header.component';

describe('AssetHeaderComponent', () => {
  let component: AssetHeaderComponent;
  let fixture: ComponentFixture<AssetHeaderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssetHeaderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssetHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
