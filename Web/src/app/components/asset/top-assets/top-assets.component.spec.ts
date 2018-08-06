import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TopAssetsComponent } from './top-assets.component';

describe('TopAssetsComponent', () => {
  let component: TopAssetsComponent;
  let fixture: ComponentFixture<TopAssetsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TopAssetsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TopAssetsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
