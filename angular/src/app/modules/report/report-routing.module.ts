import { StaffRecruitmentPerformanceComponent } from './staff-recruitment-performance/staff-recruitment-performance.component';
import { RecruitmentOverviewComponent } from './recruitment-overview/recruitment-overview.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ReportResolver } from '../../core/resolver/report.resolver';
import { InternRecruitmentPerformanceComponent } from './intern-recruitment-performance/intern-recruitment-performance.component';
import { ReportEducationComponent } from './report-education/report-education.component';

const routes: Routes = [
  { path: '', redirectTo: 'recruiment-overview' },
  {
    path: 'recruitment-overview',
    resolve: { reportResolver: ReportResolver },
    component: RecruitmentOverviewComponent
  },
  {
    path: 'staff-recruitment-performance',
    resolve: { reportResolver: ReportResolver },
    component: StaffRecruitmentPerformanceComponent
  },
  {
    path: 'intern-recruitment-performance',
    resolve: { reportResolver: ReportResolver },
    component: InternRecruitmentPerformanceComponent
  },
  {
    path: 'education-intern',
    resolve: { reportResolver: ReportResolver },
    component: ReportEducationComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportRoutingModule { }
