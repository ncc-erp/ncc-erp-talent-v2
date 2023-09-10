import { Component, Injector, OnInit } from '@angular/core';
import { Branch } from '@app/core/models/categories/branch.model';
import { ReportOverviewService } from '@app/core/services/report/report-overview.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { DateFormat } from '@shared/AppConsts';
import { CreationTimeEnum, DefaultRoute } from '@shared/AppEnums';
import { TalentDateTime } from '@shared/components/date-selector/date-selector.component';
import { SourceStatistic } from "@app/core/models/report/report-performance.model";
import { ApiResponse } from '@shared/paged-listing-component-base';
import { forkJoin, Subscription } from 'rxjs';
import * as pluginDataLabels from 'chartjs-plugin-datalabels';

@Component({
  selector: 'talent-staff-recruitment-performance',
  templateUrl: './staff-recruitment-performance.component.html',
  styleUrls: ['./staff-recruitment-performance.component.scss']
})
export class StaffRecruitmentPerformanceComponent extends AppComponentBase implements OnInit {
  isLoading: boolean;
  searchWithCreationTime: TalentDateTime;
  lengthCVSourceAndStatus: number;
  defaultOptionTime: string = CreationTimeEnum.MONTH;
  filterBranch?: Branch[] = [
    {
      id: null, name: 'All', displayName: 'All'
    } as Branch
  ];
  branches: Branch[] = [];
  reports: SourceStatistic[] = [];
  subscription: Subscription;
  data: any[] = [];
  arrColor: string[] = [];

  public chartPlugins = [pluginDataLabels.default];

  public chartOptions: any;

  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    private _report: ReportOverviewService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
    this.branches = this.getDropdownFilterBranch();
    this.arrColor = this.getColorPieChart();
    this.getPerformanceStatistics();
    this.chartOptions = this.getOptionsChart();
  }


  async getPerformanceStatistics(isChangeTime = false) {
    let promises = [];
    const fd = this.searchWithCreationTime?.fromDate.format(DateFormat.YYYY_MM_DD);
    const td = this.searchWithCreationTime?.toDate.format(DateFormat.YYYY_MM_DD);
    if (!fd) return;

    this.filterBranch.forEach((branch: Branch) => {
      if (!isChangeTime && this.reports.find(s => s.branchId == branch.id || (s.branchId == 0 && branch.id == null))) return;
      promises.push(this._report.getPerformanceStatisticStaff(fd, td, branch.id));
    });

    forkJoin(promises).subscribe((res: ApiResponse<SourceStatistic>[]) => {
      if (isChangeTime) this.reports = [];
      res.forEach((element) => {
        this.reports.push(element.result);
      });
      this.mapToDataPieChart(isChangeTime);
    })
  }

  mapToDataPieChart(isChangeTime) {
    if (isChangeTime) this.data = [];
    this.reports.forEach((element) => {
      if (!isChangeTime && this.data.findIndex(s => s.branchId == element.branchId) >= 0) return;
      this.data.push({
        branchId: element.branchId,
        branchName: element.branchName,
        dataChart:
        {
          labels: element.sourcePerformances.map(e => e.sourceName),
          datasets: [
            {
              data: element.sourcePerformances.map(e => e.totalCV),
              backgroundColor: this.arrColor,
              hoverBackgroundColor: this.arrColor
            }
          ]
        }
      })
    });
  }

  private getOptionsChart() {
    return {
      tooltips: {
        enabled: false
      },
      plugins: {
        datalabels: {
          formatter: (value, ctx) => {
            if (value <= 0) return "";
            let dataArr = ctx.chart.data.datasets[0].data;
            let percentage = this.getPercentage(value, dataArr);
            return percentage + '%';
          },
          color: '#fff',
        }
      }
    }
  }

  private getPercentage(value: number, arrObj: Array<number>) {
    let sum = arrObj.reduce((x, y) => x + y, 0);
    return (value * 100 / sum).toFixed(2);
  }

  getColorPieChart() {
    let arr = [];
    this._utilities.catCvSource.map(e => arr.push(e.colorCode))
    return arr;
  }

  onTalentDateChange(talentDateTime: TalentDateTime) {
    this.searchWithCreationTime = talentDateTime;
    this.getPerformanceStatistics(true);
  }

  onSelectChangeBranch($event) {
    if (this.checkRemoveReport($event)) return;
    this.getPerformanceStatistics();
  }

  private checkRemoveReport($event) {
    if ($event.value.length == 0) {
      this.reports = [];
      this.data = [];
      return true;
    }

    let valItemId = $event.itemValue?.id;
    let index = this.reports.findIndex(s => s.branchId == valItemId ||
      (s.branchId == 0 && valItemId == null));

    if (index < 0) return false;
    this.reports.splice(index, 1);
    this.data.splice(index, 1);
    return true;
  }

  getDropdownFilterBranch() {
    return [
      { id: null, name: 'All', displayName: 'All' } as Branch,
      ...this._utilities.catBranch
    ]
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Reports", routerLink: DefaultRoute.Report, styleClass: 'menu-item-click' }, { label: "Staff Source" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}
