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
import { BsModalService } from 'ngx-bootstrap/modal';
import { EducationStatistic } from './../../../core/models/report/report-education.model';
import { ReportInternService } from './../../../core/services/report/report-intern.service';
import { ExportInternEducationComponent } from './components/export-intern-education/export-intern-education.component';
import { BranchEducationData } from './interfaces/report-education.interface';
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
  recruitmentQuantityCharts: any[] = []; 
  recruitmentPercentageCharts: any[] = [];
  candidateQuantityData: BranchEducationData[] = [];
  candidateDensityData: BranchEducationData[] = [];
  filterModeMap: { [branchId: string]: boolean } = {};

  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    private _reportIntern: ReportInternService,
    private _modalService: BsModalService
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
    this.getCandidateDensityByEducation(true);
    this.getCandidateQuantityByEducation(true);
  }

  onSelectChangeBranch($event) {
    if (this.checkRemoveReport($event)) return;
    this.getAllInternEducationOnboard();
    this.getAllInternEducationPassTest();
    this.getCandidateDensityByEducation();
    this.getCandidateQuantityByEducation();
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

  private getCandidateQuantityByEducation(isChangeTime = false): void {
    let promises = [];
    const fd = this.searchWithCreationTime?.fromDate.format(DateFormat.YYYY_MM_DD);
    const td = this.searchWithCreationTime?.toDate.format(DateFormat.YYYY_MM_DD);

    if (!fd) return;

    this.filterBranch.forEach((branch: Branch) => {
      promises.push(this._reportIntern.getCandidateQuantityByEducation(fd, td, branch.id));
    });

    forkJoin(promises).subscribe({
      next: (res: ApiResponse<BranchEducationData>[]) => {        
        if (isChangeTime) this.candidateQuantityData = [];
        res.forEach((element, index) => {
          if (element && element.result) {
            this.candidateQuantityData[index] = element.result;
          } else {
            this.candidateQuantityData[index] = {
              branchId: this.filterBranch[index].id,
              branchName: this.filterBranch[index].displayName || this.filterBranch[index].name,
              educations: []
            };
          }
        });
        this.mapToQuantityChart(isChangeTime);
        this.mapToPercentageChart(isChangeTime);
      },
      error: (error) => {
        console.error('Error loading quantity data:', error);
      }
    });
  }

  private getCandidateDensityByEducation(isChangeTime = false): void {
    let promises = [];
    const fd = this.searchWithCreationTime?.fromDate.format(DateFormat.YYYY_MM_DD);
    const td = this.searchWithCreationTime?.toDate.format(DateFormat.YYYY_MM_DD);
    if (!fd) return;
    this.filterBranch.forEach((branch: Branch) => {
      promises.push(this._reportIntern.getCandidateDensityByEducation(fd, td, branch.id));
    });

    forkJoin(promises).subscribe({
      next: (res: ApiResponse<BranchEducationData>[]) => {        
        if (isChangeTime) this.candidateDensityData = [];
        res.forEach((element, index) => {
          if (element && element.result) {
            this.candidateDensityData[index] = element.result;
          } else {
            this.candidateDensityData[index] = {
              branchId: this.filterBranch[index].id,
              branchName: this.filterBranch[index].displayName || this.filterBranch[index].name,
              educations: []
            };
          }
        });
        this.mapToPercentageChart(isChangeTime);
      },
      error: (error) => {
        console.error('Error loading density data:', error);
      }
    });
  }

  private mapToQuantityChart(isChangeTime: boolean): void {
    if (isChangeTime) this.recruitmentQuantityCharts = [];

    if (this.candidateQuantityData.length === 0) {
      return;
    }

    this.filterBranch.forEach((branch, branchIndex) => {
      if (!isChangeTime && this.recruitmentQuantityCharts.findIndex(s => s.branchId == branch.id) >= 0) {
        const existingIndex = this.recruitmentQuantityCharts.findIndex(s => s.branchId == branch.id);
        if (existingIndex >= 0) {
          this.recruitmentQuantityCharts.splice(existingIndex, 1);
        }
      }

      const quantityBranchData = this.candidateQuantityData[branchIndex];
      if (!quantityBranchData) return;

      const quantityEducations = quantityBranchData.educations || [];
      const activeEducations = quantityEducations.filter(q => q && q.totalCV && q.totalCV > 0);
      
      if (activeEducations.length === 0) return;

      const labels = activeEducations.map(e => e.educationName);
      const datasets = [];
      
      datasets.push({
        label: 'Onboard',
        data: activeEducations.map(e => e.onboard || 0),
        backgroundColor: '#28a745',
        borderColor: '#28a745',
        borderWidth: 0,
        stack: 'stack1'
      });
      
      datasets.push({
        label: 'Pass Interview', 
        data: activeEducations.map(e => e.passInterview || 0),
        backgroundColor: '#17a2b8',
        borderColor: '#17a2b8',
        borderWidth: 0,
        stack: 'stack1'
      });
      
      datasets.push({
        label: 'Pass Test',
        data: activeEducations.map(e => e.passTest || 0),
        backgroundColor: '#ffc107',
        borderColor: '#ffc107',
        borderWidth: 0,
        stack: 'stack1'
      });
      
      datasets.push({
        label: 'Pass CV',
        data: activeEducations.map(e => e.passCV || 0),
        backgroundColor: '#fd7e14',
        borderColor: '#fd7e14',
        borderWidth: 0,
        stack: 'stack1'
      });

      this.recruitmentQuantityCharts.push({
        branchId: branch.id,
        branchName: branch.displayName || branch.name,
        dataChart: {
          labels: labels,
          datasets: datasets
        }
      });
    });
  }

  private mapToPercentageChart(isChangeTime: boolean): void {
    if (isChangeTime) this.recruitmentPercentageCharts = [];

    if (this.candidateDensityData.length === 0) {
      return;
    }

    this.filterBranch.forEach((branch, branchIndex) => {
      if (!isChangeTime && this.recruitmentPercentageCharts.findIndex(s => s.branchId == branch.id) >= 0) {
        const existingIndex = this.recruitmentPercentageCharts.findIndex(s => s.branchId == branch.id);
        if (existingIndex >= 0) {
          this.recruitmentPercentageCharts.splice(existingIndex, 1);
        }
      }

      const densityBranchData = this.candidateDensityData[branchIndex];
      const quantityBranchData = this.candidateQuantityData[branchIndex];

      if (!densityBranchData) return;

      const densityEducations = densityBranchData.educations || [];
      const activeEducations = densityEducations.filter(d => 
        d && (d.passCV > 0 || d.passTest > 0 || d.passInterview > 0 || d.onboard > 0 || d.other > 0)
      );

      if (activeEducations.length === 0) return;

      const stages = ['Pass CV', 'Pass Test', 'Pass Interview', 'Onboard'];

      this.recruitmentPercentageCharts.push({
        branchId: branch.id,
        branchName: branch.displayName || branch.name,
        dataChart: {
          labels: stages,
          datasets: activeEducations.map((education) => {
            const data = [
              education.passCV || 0,
              education.passTest || 0,
              education.passInterview || 0,
              education.onboard || 0
            ];

            const quantityEducation = quantityBranchData?.educations?.find(e => e.educationName === education.educationName);
            const counts = [
              quantityEducation?.passCV || 0,
              quantityEducation?.passTest || 0,
              quantityEducation?.passInterview || 0,
              quantityEducation?.onboard || 0
            ];

            return {
              label: education.educationName,
              data: data,
              backgroundColor: education.colorCode,
              borderColor: education.colorCode,
              borderWidth: 0,
              counts: counts
            };
          })
        }
      });
    });
  }

  getTotalCVByBranch(branchId: any): number {
    const branchData = this.candidateQuantityData.find(data => 
      data.branchId === branchId || (data.branchId === 0 && branchId === null)
    );
    
    if (!branchData || !branchData.educations) {
      return 0;
    }
    
    return branchData.educations.reduce((total, education) => {
      return total + (education.totalCV || 0);
    }, 0);
  }
  
  formatNumber(num: number): string {
    return num.toLocaleString();
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
      this.recruitmentQuantityCharts = [];
      this.recruitmentPercentageCharts = [];
      this.candidateQuantityData = [];
      this.candidateDensityData = [];
      return true;
    }

    let valItemId = $event.itemValue?.id;
    let index = this.educationOnboarded.findIndex(s => s.branchId == valItemId || (s.branchId == 0 && valItemId == null));

    if (index < 0) return false;
    this.dataPieChart.splice(index, 1);
    this.dataBarChart.splice(index, 1);
    this.educationOnboarded.splice(index, 1);
    this.educationPassTest.splice(index, 1);
    this.recruitmentQuantityCharts.splice(index, 1);
    this.recruitmentPercentageCharts.splice(index, 1);
    this.candidateQuantityData.splice(index, 1);
    this.candidateDensityData.splice(index, 1);
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

  getQuantityChartByBranch(branchId: any): any {
    return this.recruitmentQuantityCharts.find(chart => chart.branchId === branchId);
  }

  getPercentageChartByBranch(branchId: any): any {
    return this.recruitmentPercentageCharts.find(chart => chart.branchId === branchId);
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

  exportInternEducation() {
    const fd = this.searchWithCreationTime?.fromDate.format('YYYY-MM-DD');
    const td = this.searchWithCreationTime?.toDate.format('YYYY-MM-DD');
    const branches = this.filterBranch.map(branch => {
      return { id: branch.id !== null ? branch.id : "", displayName: branch.displayName };
    });

    if (branches.length === 0) {
      this.showToastMessage(ToastMessageType.ERROR, "Please select branch");
      return;
    }

    this._modalService.show(ExportInternEducationComponent, {
      class: 'modal-lg',
      initialState: {
        fromDate: fd,
        toDate: td,
        branchs: branches
      }
    });
  }
}