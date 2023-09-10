import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NccCvComponent } from './ncc-cv.component';

describe('NccCvComponent', () => {
  let component: NccCvComponent;
  let fixture: ComponentFixture<NccCvComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ NccCvComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(NccCvComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
