import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InternRecruitmentPerformanceComponent } from './intern-recruitment-performance.component';

describe('InternRecruitmentPerformanceComponent', () => {
  let component: InternRecruitmentPerformanceComponent;
  let fixture: ComponentFixture<InternRecruitmentPerformanceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InternRecruitmentPerformanceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InternRecruitmentPerformanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
