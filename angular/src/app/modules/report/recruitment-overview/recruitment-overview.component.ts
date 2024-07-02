import { LoopArrayPipe } from "./../../../../shared/pipes/loop-array.pipe";
import { ToastMessageType } from "./../../../../shared/AppEnums";
import { Observable, Subject } from "rxjs";
import {
  OverviewStatistic,
  RecruitmentOverview,
  RecruitmentOverviewRequest,
} from "../../../core/models/report/report.model";
import { ReportOverviewService } from "../../../core/services/report/report-overview.service";
import { Injector, OnDestroy } from "@angular/core";
import { AppComponentBase } from "@shared/app-component-base";
import { CreationTimeEnum, DefaultRoute } from "@shared/AppEnums";
import { Component, OnInit } from "@angular/core";
import { UtilitiesService } from "@app/core/services/utilities.service";
import { TalentDateTime } from "@shared/components/date-selector/date-selector.component";
import { DateFormat } from "@shared/AppConsts";
import { LoopNumberPipe } from "@shared/pipes/loop-number.pipe";
import { Branch } from "@app/core/models/categories/branch.model";
import { map, switchMap, takeUntil } from "rxjs/operators";
import { CatalogModel } from "@app/core/models/common/common.dto";
import { ExportDialogComponent } from "../../../../shared/components/export-dialog/export-dialog.component";
import { MatDialog } from "@angular/material/dialog";
import { ExportDialogService } from "../../../core/services/export/export-dialog.service";

@Component({
  selector: "talent-recruitment-overview",
  templateUrl: "./recruitment-overview.component.html",
  styleUrls: ["./recruitment-overview.component.scss"],
  providers: [LoopNumberPipe, LoopArrayPipe],
})
export class RecruitmentOverviewComponent
  extends AppComponentBase
  implements OnInit, OnDestroy
{
  private getRecruitmentOverview$: Subject<void> = new Subject<void>();
  private destroy$: Subject<void> = new Subject<void>();
  isLoading: boolean;
  public overviewReport: RecruitmentOverview[] = [];
  defaultOptionTime: string = CreationTimeEnum.MONTH;
  filterDate: TalentDateTime;
  filterBranches: Branch[] = [
    {
      id: null,
      name: "All",
      displayName: "All",
    } as Branch,
  ];
  filterCandidateTypes: CatalogModel = {
    id: null,
    name: "All",
  };
  filterAssignTo: CatalogModel = { id: null, name: "All" };
  branches: Branch[] = [];
  candidateTypes: CatalogModel[] = [];
  userList: CatalogModel[] = [];
  isDialogOpen = false;
  public cvStatusHeaders: CatalogModel[] = [];
  public cvSourceHeaders: CatalogModel[] = [];
  public candidateStatusHeaders: CatalogModel[] = [];
  public hasNewCVStatus: boolean = false;

  constructor(
    injector: Injector,
    public _utilitiesService: UtilitiesService,
    private _reportService: ReportOverviewService,
    public dialogService: MatDialog,
    private _exportService: ExportDialogService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
    this.branches = this.getDropdownFilterBranch();
    this.candidateTypes = this.getDropdownFilterUserType();
    this.getDropdownFilterUser().subscribe((users) => {
      this.userList = users;
    });
    this.getRecruitmentOverviewStatistics();
  }

  onChangeFilterCandidateTypes() {
    this.getRecruitmentOverview$.next();
  }

  onChangeFilterAssignTo() {
    this.getRecruitmentOverview$.next();
  }

  onChangeFilterBranches() {
    this.getRecruitmentOverview$.next();
  }

  onChangeFilterDate(talentDateTime: TalentDateTime) {
    this.filterDate = talentDateTime;
    this.getRecruitmentOverview$.next();
  }

  private getRecruitmentOverviewStatistics() {
    this.getRecruitmentOverview$
      .pipe(
        takeUntil(this.destroy$),
        switchMap(() => {
          this.isLoading = true;
          const branchIds = this.filterBranches.map((branch) => branch.id);
          const payload: RecruitmentOverviewRequest = {
            fromDate: this.filterDate?.fromDate.toDate(),
            toDate: this.filterDate?.toDate.toDate(),
            userType: this.filterCandidateTypes.id,
            isGetAllBranch: branchIds.includes(null),
            branchIds: branchIds.filter((id) => id),
            userId: this.filterAssignTo.id,
          };
          return this._reportService.getRecruitmentOverview(payload);
        })
      )
      .subscribe((res) => {
        this.overviewReport = res.result;
        this.resetHeaderItems();
        this.getHeaderItems();
        this.isLoading = false;
      });
  }

  private getHeaderItems() {
    this.overviewReport?.forEach((branch) => {
      branch.subPositionStatistics?.forEach((subPosition) => {
        subPosition.cvStatusStatistics?.forEach((cvStatus) => {
          if (!this.cvStatusHeaders.some((item) => item.id === cvStatus.id))
            this.cvStatusHeaders.push({
              id: cvStatus.id,
              name: cvStatus.name,
            });
        });
        subPosition.cvSourceStatistics?.forEach((cvSource) => {
          if (
            !this.cvSourceHeaders.some((item) => item.id === cvSource.id) &&
            cvSource.id
          )
            this.cvSourceHeaders.push({
              id: cvSource.id,
              name: cvSource.name,
            });
        });
        subPosition.candidateStatusStatistics?.forEach((candidateStatus) => {
          if (
            !this.candidateStatusHeaders.some(
              (item) => item.id === candidateStatus.id
            )
          )
            this.candidateStatusHeaders.push({
              id: candidateStatus.id,
              name: candidateStatus.name,
            });
        });
      });
    });
    this.cvStatusHeaders.sort((a, b) => a.id - b.id);
    this.candidateStatusHeaders.sort((a, b) => a.id - b.id);
    this.hasNewCVStatus = this.cvStatusHeaders.some(
      (cvStatus) => cvStatus.id == 0
    );
  }

  private resetHeaderItems() {
    this.cvStatusHeaders = [];
    this.cvSourceHeaders = [];
    this.candidateStatusHeaders = [];
  }

  public getQuantityById(list: OverviewStatistic[], id: number): number {
    return list?.find((item) => item.id === id)?.quantity;
  }

  public getPreviousQuantityById(list: OverviewStatistic[], id: number): number {
    return list?.find((item) => item.id === id)?.previousQuantity;
  }

  public getCurrentQuantityById(list: OverviewStatistic[], id: number): number {
    return list?.find((item) => item.id === id)?.currentQuantity;
  }

  getDropdownFilterBranch() {
    return [
      { id: null, name: "All", displayName: "All" } as Branch,
      ...this._utilitiesService.catBranch,
    ];
  }

  getDropdownFilterUserType() {
    return [
      {
        id: null,
        name: "All",
      } as CatalogModel,
      ...this._utilitiesService.catUserType,
    ];
  }
  getDropdownFilterUser(): Observable<CatalogModel[]> {
    return this._reportService.getAllUserCreated().pipe(
      map((res) => {
        if (res?.result) {
          const users = res.result;
          users.unshift({ id: null, name: "All" } as CatalogModel);
          return users;
        } else {
          return [];
        }
      })
    );
  }

  getLengthCVSourceAndStatus() {
    return (
      this._utilitiesService.catCvSource.length +
      this._utilitiesService.catReqCvStatus.length
    );
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [
        {
          label: "Reports",
          routerLink: DefaultRoute.Report,
          styleClass: "menu-item-click",
        },
        { label: "Recruitment Overview" },
      ],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

  exportOverviewHiring() {
    const dialogConfig = {
      hasBackdrop: false,
      position: {
        top: "40%",
        right: "50px",
      },
      panelClass: "custom-dialog",
    };
    const userType = this.filterCandidateTypes?.id;
    const fd = this.filterDate?.fromDate.format(DateFormat.YYYY_MM_DD);
    const td = this.filterDate?.toDate.format(DateFormat.YYYY_MM_DD);
    const branches = this.filterBranches.map((branch) => {
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
      const popup = this.dialogService.open(
        ExportDialogComponent,
        dialogConfig
      );
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

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
    this.getRecruitmentOverview$.complete();
  }
}
