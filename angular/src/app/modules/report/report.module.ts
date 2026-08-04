import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportRoutingModule } from './report-routing.module';
import { RecruitmentOverviewComponent } from './recruitment-overview/recruitment-overview.component';
import { SharedModule } from '@shared/shared.module';
import { ChartModule } from 'primeng/chart';
import { StaffRecruitmentPerformanceComponent } from './staff-recruitment-performance/staff-recruitment-performance.component';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { InternRecruitmentPerformanceComponent } from './intern-recruitment-performance/intern-recruitment-performance.component';
import { ReportEducationComponent } from './report-education/report-education.component';
import { PercentageChartComponent } from './report-education/components/percentage-chart.component'
import { QuantityChartComponent } from './report-education/components/quantity-chart.component'
import { ExportInternEducationComponent } from './report-education/components/export-intern-education/export-intern-education.component';
@NgModule({
  declarations: [
    RecruitmentOverviewComponent,
    StaffRecruitmentPerformanceComponent,
    InternRecruitmentPerformanceComponent,
    ReportEducationComponent,
    PercentageChartComponent,
    QuantityChartComponent,
    ExportInternEducationComponent,
  ],
  imports: [
    CommonModule,
    ReportRoutingModule,
    SharedModule,
    ChartModule,
    ProgressSpinnerModule
  ]
})
export class ReportModule { }
