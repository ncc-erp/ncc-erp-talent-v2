import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectStaffComponent } from './project-staff.component';

describe('ProjectStaffComponent', () => {
  let component: ProjectStaffComponent;
  let fixture: ComponentFixture<ProjectStaffComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProjectStaffComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProjectStaffComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
