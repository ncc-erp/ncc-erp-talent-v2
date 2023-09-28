import { Component, Injector, OnInit } from '@angular/core';
import { Branch } from '@app/core/models/categories/branch.model';
import { BreadCrumbConfig } from '@app/core/models/common/common.dto';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { DateFormat } from '@shared/AppConsts';
import { CreationTimeEnum, DefaultRoute, ToastMessageType } from '@shared/AppEnums';
import { TalentDateTime } from '@shared/components/date-selector/date-selector.component';
import { NccAppComponentBase } from '@shared/ncc-component-base';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { ChartOptions } from 'chart.js';
import * as pluginDataLabels from 'chartjs-plugin-datalabels';
import { forkJoin } from 'rxjs';
import { EducationStatistic } from './../../../core/models/report/report-education.model';
import { ReportInternService } from './../../../core/services/report/report-intern.service';
import {ExportDialogComponent} from 'export-dialog/export-dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {ExportDialogService} from 'export-dialog/export-dialog.service';

@Component({
  selector: 'talent-report-education',
  templateUrl: './report-education.component.html',
  styleUrls: ['./report-education.component.scss']
})
export class ReportEducationComponent extends NccAppComponentBase implements OnInit {

  searchWithCreationTime: TalentDateTime;
  defaultOptionTime: string = CreationTimeEnum.MONTH;

  branches: Branch[] = [];
  filterBranch?: Branch[] = [
    {
      id: null, name: 'All', displayName: 'All'
    } as Branch
  ];

  dataPieChart: any[] = [];
  dataBarChart: any[] = [];
  educationOnboarded: EducationStatistic[] = [];
  educationPassTest: EducationStatistic[] = [];

  public chartPlugins = [pluginDataLabels.default];
  public chartOptionsPie: ChartOptions;
  public chartOptionsBar: ChartOptions;
  isDialogOpen = false;

  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    private _reportIntern: ReportInternService,
    public dialogService: MatDialog,
    private _exportService: ExportDialogService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.setDropdownFilterBranch();
    this.setOptionsCharts();
  }

  onTalentDateChange(talentDateTime: TalentDateTime) {
    this.searchWithCreationTime = talentDateTime;
    this.getAllInternEducationOnboard(true);
    this.getAllInternEducationPassTest(true);
  }

  onSelectChangeBranch($event) {
    if (this.checkRemoveReport($event)) return;
    this.getAllInternEducationOnboard();
    this.getAllInternEducationPassTest();
  }

  private getAllInternEducationOnboard(isChangeTime = false): void {
    let promises = [];
    const fd = this.searchWithCreationTime?.fromDate.format(DateFormat.YYYY_MM_DD);
    const td = this.searchWithCreationTime?.toDate.format(DateFormat.YYYY_MM_DD);

    if (!fd) return;

    this.filterBranch.forEach((branch: Branch) => {
      if (!isChangeTime && this.educationOnboarded.find(s => s.branchId == branch.id || (s.branchId == 0 && branch.id == null))) return;
      promises.push(this._reportIntern.getEducationInternOnboarded(fd, td, branch.id));
    });

    forkJoin(promises).subscribe((res: ApiResponse<EducationStatistic>[]) => {
      if (isChangeTime) this.educationOnboarded = [];
      res.forEach((element) => {
        this.educationOnboarded.push(element.result);
      });
      this.mapToDataPieChart(isChangeTime);
    })
  }

  private getAllInternEducationPassTest(isChangeTime = false): void {
    let promises = [];
    const fd = this.searchWithCreationTime?.fromDate.format(DateFormat.YYYY_MM_DD);
    const td = this.searchWithCreationTime?.toDate.format(DateFormat.YYYY_MM_DD);

    if (!fd) return;

    this.filterBranch.forEach((branch: Branch) => {
      if (!isChangeTime && this.educationPassTest.find(s => s.branchId == branch.id || (s.branchId == 0 && branch.id == null))) return;
      promises.push(this._reportIntern.getEducationPassTest(fd, td, branch.id));
    });

    forkJoin(promises).subscribe((res: ApiResponse<EducationStatistic>[]) => {
      if (isChangeTime) this.educationPassTest = [];
      res.forEach((element) => {
        this.educationPassTest.push(element.result);
      });
      this.mapToDataBarChart(isChangeTime);
    })
  }

  private mapToDataBarChart(isChangeTime: boolean): void {
    if (isChangeTime) this.dataBarChart = [];
    this.educationPassTest.forEach((item) => {
      if (!isChangeTime && this.dataBarChart.findIndex(s => s.branchId == item.branchId) >= 0) return;
      this.dataBarChart.push({
        branchId: item.branchId,
        branchName: item.branchName,
        dataChart:
        {
          labels: [this.searchWithCreationTime.dateText],
          datasets: item.educations.map(education => {
            return {
              label: education.educationName,
              data: [education.totalCV],
              backgroundColor: education.colorCode,
              hoverBackgroundColor: education.colorCode,
              totalCvs: item.educations.map(obj => obj.totalCV),
            };
          })
        }
      })
    });
  }

  private mapToDataPieChart(isChangeTime: boolean): void {
    if (isChangeTime) this.dataPieChart = [];
    this.educationOnboarded.forEach((item) => {
      if (!isChangeTime && this.dataPieChart.findIndex(s => s.branchId == item.branchId) >= 0) return;

      const colorArr = item.educations.map(e => e.colorCode);
      this.dataPieChart.push({
        branchId: item.branchId,
        branchName: item.branchName,
        dataChart:
        {
          labels: item.educations.map(e => e.educationName),
          datasets: [
            {
              data: item.educations.map(e => e.totalCV),
              backgroundColor: colorArr,
              hoverBackgroundColor: colorArr
            }
          ]
        }
      })
    });
  }

  private checkRemoveReport($event) {
    if ($event.value.length == 0) {
      this.dataPieChart = [];
      this.dataBarChart = [];
      this.educationOnboarded = [];
      this.educationPassTest = [];
      return true;
    }

    let valItemId = $event.itemValue?.id;
    let index = this.educationOnboarded.findIndex(s => s.branchId == valItemId || (s.branchId == 0 && valItemId == null));

    if (index < 0) return false;
    this.dataPieChart.splice(index, 1);
    this.dataBarChart.splice(index, 1);
    this.educationOnboarded.splice(index, 1);
    this.educationPassTest.splice(index, 1);
    return true;
  }

  private setDropdownFilterBranch(): void {
    this.branches = [
      { id: null, name: 'All', displayName: 'All' } as Branch,
      ...this._utilities.catBranch
    ]
  }

  private setOptionsCharts(): void {
    const formatLabelBarChart = (value, ctx) => {
      if (value <= 0) return "";
      const dataArr = ctx.chart.data.datasets[0].totalCvs;
      const percentage = this.getPercentage(value, dataArr);
      return percentage + '%';
    };

    const formatLabelPieChart = (value, ctx) => {
      if (value <= 0) return "";
      const dataArr = ctx.chart.data.datasets[0].data;
      const percentage = this.getPercentage(value, dataArr);
      return percentage + '%';
    };

    this.chartOptionsPie = this.getOptionsChart(formatLabelPieChart);
    this.chartOptionsBar = this.getOptionsChart(formatLabelBarChart);
  }

  private getOptionsChart(funcFormatter: Function) {
    return {
      tooltips: {
        enabled: false
      },
      plugins: {
        datalabels: {
          formatter: (value, ctx) => funcFormatter(value, ctx),
          color: '#fff',
        },
        legend: {
          display: true,
          labels: {
            usePointStyle: true,
            pointStyle: 'rect'
          }
        },
      },
      datasets: {
        bar: {
          barPercentage: 0.5,
        }
      }
    }
  }

  private getPercentage(value: number, arrObj: Array<number>, sumAvailabel?: number) {
    let sum = arrObj.reduce((x, y) => x + y, 0);
    return (value * 100 / sum).toFixed(2);
  }

  protected getBreadCrumbConfig(): BreadCrumbConfig {
    return this.breadcrumbConfig = {
      menuItem: [{ label: "Reports", routerLink: DefaultRoute.Report, styleClass: 'menu-item-click' }, { label: "Education" }],
      homeItem: this.homeMenuItem,
    };
  }

  isShowExportBtn(){
    return this.isGranted(this.PS.Pages_Reports_Intern_Education_Export);
  }

  exportInternEducation() {
    const dialogConfig = {
      hasBackdrop: false,
      position: {
        top: "48em",
        right: "50px",
      },
      panelClass: "custom-dialog",
    };
    const fd = this.searchWithCreationTime?.fromDate.format(DateFormat.YYYY_MM_DD);
    const td = this.searchWithCreationTime?.toDate.format(DateFormat.YYYY_MM_DD);
    const branches = this.filterBranch.map(branch => {
      return { id: branch.id !== null ? branch.id : "", displayName: branch.displayName };
    });
    if (branches.length === 0 ) {
      this.showToastMessage(ToastMessageType.ERROR, "Please select branch");
      this.isDialogOpen = false;
      return;
    }
      if (!this.isDialogOpen) {
      const modalPopup= this.dialogService.open(ExportDialogComponent,dialogConfig)
      const sendataIntern =
      {
        fromDate: fd,
        toDate: td,
        branchs: branches,
      }
      this._exportService.exportInternEducation(sendataIntern);

      setTimeout(() => {
        modalPopup.close();
      }, 5000);
    }
  }
}
