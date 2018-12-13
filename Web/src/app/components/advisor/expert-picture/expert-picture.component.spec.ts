import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpertPictureComponent } from './expert-picture.component';

describe('ExpertPictureComponent', () => {
  let component: ExpertPictureComponent;
  let fixture: ComponentFixture<ExpertPictureComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpertPictureComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpertPictureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
