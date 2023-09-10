import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffRecruitmentPerformanceComponent } from './staff-recruitment-performance.component';

describe('StaffRecruitmentPerformanceComponent', () => {
  let component: StaffRecruitmentPerformanceComponent;
  let fixture: ComponentFixture<StaffRecruitmentPerformanceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StaffRecruitmentPerformanceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffRecruitmentPerformanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
