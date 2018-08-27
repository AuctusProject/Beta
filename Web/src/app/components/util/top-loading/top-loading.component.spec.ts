import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TopLoadingComponent } from './top-loading.component';

describe('TopLoadingComponent', () => {
  let component: TopLoadingComponent;
  let fixture: ComponentFixture<TopLoadingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TopLoadingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TopLoadingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
