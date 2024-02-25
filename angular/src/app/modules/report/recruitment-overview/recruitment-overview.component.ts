import { LoopArrayPipe } from './../../../../shared/pipes/loop-array.pipe';
import { REQUEST_CV_STATUS, ToastMessageType, } from './../../../../shared/AppEnums';
import { Observable, forkJoin, of } from 'rxjs';
import { ReportOverview, StatusStatistic } from '../../../core/models/report/report.model';
import { ReportOverviewService } from '../../../core/services/report/report-overview.service';
import { Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { CreationTimeEnum, DefaultRoute } from '@shared/AppEnums';
import { Component, OnInit } from '@angular/core';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { TalentDateTime } from '@shared/components/date-selector/date-selector.component';
import { DateFormat } from '@shared/AppConsts';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { LoopNumberPipe } from '@shared/pipes/loop-number.pipe';
import { Branch } from '@app/core/models/categories/branch.model';
import { catchError, map } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';
import { CatalogModel } from '@app/core/models/common/common.dto';
import { ExportDialogComponent } from '../../../../shared/components/export-dialog/export-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { ExportDialogService } from '../../../core/services/export/export-dialog.service';

@Component({
  selector: 'talent-recruitment-overview',  
  templateUrl: './recruitment-overview.component.html',
  styleUrls: ['./recruitment-overview.component.scss'],
  providers: [LoopNumberPipe, LoopArrayPipe]
})
export class RecruitmentOverviewComponent extends AppComponentBase implements OnInit {
  reports: ReportOverview[] = [];
  isLoading: boolean;
  searchWithCreationTime: TalentDateTime;
  lengthCVSourceAndStatus: number;
  defaultOptionTime: string = CreationTimeEnum.MONTH;
  filterBranch?: Branch[] = [
    {
      id: null, name: 'All', displayName: 'All'
    } as Branch
  ];
  filterUserType: CatalogModel = { id: null, name: 'All', } as CatalogModel;
  filterUser: CatalogModel = { id: null, name: 'All', } as CatalogModel;

  branches: Branch[] = [];
  usertypes: CatalogModel[] = [];
  userList: CatalogModel[] = [];
  colOverview: number;
  colQuantity: number = 5; //Col: QuatyHiring, QuatyApply, QuatyFailed, ...
  colLabel: number = 2; //Col: No, PositionName
  isDialogOpen = false;
  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    private _report: ReportOverviewService,
    public dialogService: MatDialog,
    private _exportService: ExportDialogService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
    this.branches = this.getDropdownFilterBranch();
    this.usertypes = this.getDropdownFilterUserType();
    this.getDropdownFilterUser().subscribe(users => {
      this.userList = users;
    });
    this.lengthCVSourceAndStatus = this.getLengthCVSourceAndStatus();
    this.colOverview = this.getColTotalOverview();
  }

  public lazyLoadingData($event) {
    this.getOverviewStatistics();
  }

  onSelectChangeUserType($event) {
    this.getOverviewStatistics(false, true);
  }

  onSelectChangeUser($event) {
    this.getOverviewStatistics(false, true);
  }

  onSelectChangeBranch($event) {
    if (this.checkRemoveReport($event)) return;
    this.getOverviewStatistics();
  }

  private checkRemoveReport($event) {
    if ($event.value.length == 0) {
      this.reports = [];
      return true;
    }

    let valItemId = $event.itemValue?.id;
    let index = this.reports.findIndex(s => s.branchId == valItemId ||
      (s.branchId == 0 && valItemId == null));

    if (index < 0) return false;
    this.reports.splice(index, 1);
    return true;
  }

  getOverviewStatistics(isChangeTime = false, isChangeUserType = false) {
    let promises = [];
    const fd = this.searchWithCreationTime?.fromDate.format(DateFormat.YYYY_MM_DD);
    const td = this.searchWithCreationTime?.toDate.format(DateFormat.YYYY_MM_DD);

    this.filterBranch.forEach((branch: Branch) => {
      if (!isChangeTime && !isChangeUserType && this.reports.find(s => s.branchId == branch.id)) return;
      promises.push(this._report.getOverviewStatistic(fd, td, branch.id, this.filterUserType.id, this.filterUser?.id));
    });
    this.isLoading = true;
    forkJoin(promises)
      .pipe(
        catchError((err: HttpErrorResponse) => {
          return of({ error: err.error.error });
        })
      )
      .subscribe((res: ApiResponse<ReportOverview>[]) => {
        if (isChangeTime || isChangeUserType) this.reports = [];
        res.forEach((element) => {
          this.reports.push(element.result)
        });
        this.isLoading = false;
      });
  }

  getDropdownFilterBranch() {
    return [
      { id: null, name: 'All', displayName: 'All' } as Branch,
      ...this._utilities.catBranch
    ]
  }

  getDropdownFilterUserType() {
    return [
      {
        id: null, name: 'All',
      } as CatalogModel,
      ...this._utilities.catUserType
    ]
  }
  getDropdownFilterUser(): Observable<CatalogModel[]> {
    return this._report.getAllUserCreated().pipe(
        map(res => {
          if (res?.result) {
            const users = res.result;
            users.unshift({ id: null, name: 'All' } as CatalogModel);
          return users; 
            } else {
                return []; 
            }
        }),
    );
}

  onTalentDateChange(talentDateTime: TalentDateTime) {
    this.searchWithCreationTime = talentDateTime;
    this.getOverviewStatistics(true);
  }

  getLengthCVSourceAndStatus() {
    return this._utilities.catCvSource.length + this._utilities.catReqCvStatus.length;
  }

  getColTotalOverview() {
    return this.lengthCVSourceAndStatus + this.colQuantity;
  }

  getColorStatus(id: REQUEST_CV_STATUS) {
    if (id == REQUEST_CV_STATUS.AcceptedOffer || id == REQUEST_CV_STATUS.RejectedOffer)
      return 'text-bold';

    else if (id == REQUEST_CV_STATUS.Onboarded)
      return 'text-success text-bold';
  }

  getValueOrNone(value: number) {
    if (value == 0) return '';
    return value;
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Reports", routerLink: DefaultRoute.Report, styleClass: 'menu-item-click' }, { label: "Recruitment Overview" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }


  exporOverviewHiring() {
    const dialogConfig = {
      hasBackdrop: false,
      position: {
        top: "40%",
        right: "50px",
      },
      panelClass: "custom-dialog",
    };
    const userType = this.filterUserType?.id;
    const fd = this.searchWithCreationTime?.fromDate.format(
      DateFormat.YYYY_MM_DD
    );
    const td = this.searchWithCreationTime?.toDate.format(
      DateFormat.YYYY_MM_DD
    );
    const branches = this.filterBranch.map((branch) => {
      return {
        id: branch.id !== null ? branch.id : "",
        displayName: branch.displayName,
      };
    });
    if (branches.length === 0) {
      this.showToastMessage(ToastMessageType.ERROR, "Please select branch");
      this.isDialogOpen = false;
      return;
    }
    if (!this.isDialogOpen) {
      const popup = this.dialogService.open(ExportDialogComponent, dialogConfig);
      const sendataOverview = {
        fromDate: fd,
        toDate: td,
        userType: userType,
        branchs: branches,
      };
      this._exportService.exportOverviewHiring(sendataOverview);

      setTimeout(() => {
        popup.close();
      }, 5000);
    }
  }
}
