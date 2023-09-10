import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectStaffFakeComponent } from './project-staff-fake.component';

describe('ProjectStaffFakeComponent', () => {
  let component: ProjectStaffFakeComponent;
  let fixture: ComponentFixture<ProjectStaffFakeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProjectStaffFakeComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProjectStaffFakeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
