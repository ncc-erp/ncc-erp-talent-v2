import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RecruitmentOverviewComponent } from './recruitment-overview.component';

describe('RecruitmentOverviewComponent', () => {
  let component: RecruitmentOverviewComponent;
  let fixture: ComponentFixture<RecruitmentOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RecruitmentOverviewComponent]
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RecruitmentOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
