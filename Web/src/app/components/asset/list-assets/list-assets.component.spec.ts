import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListAssetsComponent } from './list-assets.component';

describe('ListAssetsComponent', () => {
  let component: ListAssetsComponent;
  let fixture: ComponentFixture<ListAssetsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListAssetsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListAssetsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
