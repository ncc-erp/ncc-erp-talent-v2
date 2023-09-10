import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DialogWorkingExperienceComponent } from './dialog-working-experience.component';

describe('DialogWorkingExperienceComponent', () => {
  let component: DialogWorkingExperienceComponent;
  let fixture: ComponentFixture<DialogWorkingExperienceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DialogWorkingExperienceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DialogWorkingExperienceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
