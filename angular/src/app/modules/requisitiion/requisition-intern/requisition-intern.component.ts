import { CloneAllDialogComponent } from './clone-all-dialog/clone-all-dialog.component';
import { Component, Injector, OnDestroy, OnInit, Optional } from '@angular/core';
import { checkNumber } from '@app/core/helpers/utils.helper';
import { CandidateInfo } from '@app/core/models/candidate/candidate.model';
import { RequisitionCloseAndClone, RequisitionIntern, RequisitionInternConfigDiaLog, RequisitionPayloadList, RequisitonCandidate } from '@app/core/models/requisition/requisition.model';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { CandidateInternListComponent } from '@app/modules/candidate/candidate-intern-list/candidate-intern-list.component';
import { DateFormat, FILTER_TIME, MESSAGE } from '@shared/AppConsts';
import { ActionEnum, API_RESPONSE_STATUS, COMPARISION_OPERATOR, CreationTimeEnum, DefaultRoute, SearchType, SortType, StatusEnum, ToastMessageType, UserType } from '@shared/AppEnums';
import { ApiResponse, Filter, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { MenuItem } from 'primeng/api';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { RequisitionInternService } from './../../../core/services/requisition/requisition-intern.service';
import { RequisitionInternDialogComponent } from './requisition-intern-dialog/requisition-intern-dialog.component';
import { ConfirmPresentForHr } from '@shared/pages/create-candidate/current-requisition/confirm-presentforhr/confirm-presentforhr.component';
import { TalentDateTime } from '@shared/components/date-selector/date-selector.component';

@Component({
  selector: 'talent-requisition-intern',
  templateUrl: './requisition-intern.component.html',
  styleUrls: ['./requisition-intern.component.scss']
})
export class RequisitionInternComponent extends PagedListingComponentBase<RequisitionIntern> implements OnInit, OnDestroy {
  public readonly SORT_TYPE = SortType;
  public readonly DATE_FORMAT = DateFormat;
  public readonly FILTER_TIME = FILTER_TIME;
  searchWithProcessCvStatus: number;
  reqInterns: RequisitionIntern[] = [];
  totalQuantity: number;
  currentReqIntern: RequisitionIntern = null;
  searchWithBranch: number;
  searchWithSubPosition: number;
  searchWithSkills: number[];
  searchWithReqStatus: number = 0;
  searchWithUpdatedTime: TalentDateTime = null
  isSearchAnd: boolean = false;
  isRowExpand: boolean = false;
  expandedRows: number[] = [];

  catalogConfig = {
    labelName: 'Skill',
    catalogList: this._utilities.catSkill,
    optionLabel: 'name',
    optionValue: 'id'
  }

  //Select mode
  isDialogMode: boolean;

  constructor(
    injector: Injector,
    @Optional() public config: DynamicDialogConfig,
    @Optional() public ref: DynamicDialogRef,
    public _utilities: UtilitiesService,
    private _reqIntern: RequisitionInternService,
    private _dialog: DialogService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
    this.isDialogMode = !!this.config?.data?.dialogMode;
  }

  ngOnDestroy() {
    super.ngOnDestroy();
    if (this.ref) this.ref.close()
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload = this.getPayLoad(request);
    this.subs.add(
      this._reqIntern.getAllPagging(payload).subscribe((rs) => {
        this.reqInterns = [];
        this.resetRowExpand();
        if (rs.success) {
          this.reqInterns = rs.result.items;
          this.totalQuantity = rs.result.totalQuantity;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(entity: RequisitionIntern): void {
    const deleteRequest = this._reqIntern.delete(entity.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, `#${entity.id}`).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  onCandidateSelectedRequisiton(entity: RequisitionIntern) {
    this.ref.close( entity );
  }

  onPositionSelect(subPositionId: number) {
    this.searchWithSubPosition = subPositionId;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  getCandidateInfo(candidate: RequisitonCandidate) {
    const candidateInfo: CandidateInfo = {
      ...candidate,
      id: candidate.cvId,
      cvSkills: candidate.skills
    }
    return candidateInfo;
  }

  openDialog(dialogAction: ActionEnum, reqCloseAndClone: RequisitionCloseAndClone = null) {
    const overflowOption = dialogAction !== ActionEnum.CLOSE_AND_CLONE ? 'visible' : 'auto';
    const dialogConfig: RequisitionInternConfigDiaLog = { requisitionIntern: this.currentReqIntern, action: dialogAction, reqCloseAndClone };
    const dialogRef = this._dialog.open(RequisitionInternDialogComponent, {
      header: `${dialogAction} Requisition`,
      width: "55%",
      contentStyle: { "max-height": "100%", overflow: overflowOption },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<RequisitionIntern>) => {
      if (res) {
        this.onGetDataChange();
        if (dialogConfig.action === ActionEnum.UPDATE) {
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);
          return;
        }
        this.showToastMessage(ToastMessageType.SUCCESS,
          dialogConfig.action === ActionEnum.CREATE ? MESSAGE.CREATE_SUCCESS : MESSAGE.CLONE_SUCCESS
        );
      }
    });
  }

  onGetDataChange() {
    this.resetRowExpand();
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  getCandidateRowExpand(entity: RequisitionIntern) {
    this.subs.add(
      this._reqIntern.getCVsByRequestId(entity.id).subscribe((rs) => {
        if (!rs.loading && rs.success) {
          const index = this.reqInterns.findIndex(req => req.id === entity.id);
          if (this.searchWithProcessCvStatus) {
            this.reqInterns[index].reqCvs = rs.result.filter(s => s.processCVStatus == this.searchWithProcessCvStatus) as RequisitonCandidate[];
          }
          else {
            this.reqInterns[index].reqCvs = rs.result;
          }
        }
        this.isLoading = rs.loading;
      })
    );
  }

  onCloseCloneAll() {
    const dialogRef = this._dialog.open(CloneAllDialogComponent, {
      header: `Close & Clone all requisition`,
      width: "65%",
      contentStyle: { "max-height": "100%", overflow: 'visible' },
      baseZIndex: 5000,
      data: null,
    });
    dialogRef.onClose.subscribe((res: boolean) => res && this.onGetDataChange());
  }

  onSkillChange(event) {
    this.searchWithSkills = event;
    this.onGetDataChange();
  }

  onSearchTypeChange(value: string) {
    this.isSearchAnd = value === SearchType.AND;
    this.onGetDataChange();
  }

  deleteRequestCv(entity: RequisitonCandidate, requisitionIntern: RequisitionIntern) {
    this.currentReqIntern = requisitionIntern;
    this.subs.add(
      this._reqIntern.deleteRequestCV(entity.id, requisitionIntern.id).subscribe((res) => {
        this.isLoading = res.loading;
        if (!res.loading && res.success) {
          this.updateNewRequisiton(res.result);
          this.getCandidateRowExpand(this.currentReqIntern);
          this.showToastMessage(ToastMessageType.SUCCESS, `Delete ${entity.fullName} successfully`);
        }
      })
    );
  }

  onClose(entity: RequisitionIntern) {
    this.subs.add(
      this._reqIntern.closeRequisition(entity.id).subscribe((rs) => {
        if (!rs.loading && rs.success) {
          this.refresh();
          this.showToastMessage(ToastMessageType.WARN, MESSAGE.CLOSE_SUCCESS);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  onCloseAndClone(entity: RequisitionIntern) {
    this.subs.add(
      this._reqIntern.getRequisitionToCloseAndClone(entity.id).subscribe((rs) => {
        if (!rs.loading && rs.success) {
          this.openDialog(this.DIALOG_ACTION.CLOSE_AND_CLONE, rs.result);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  onReopen(entity: RequisitionIntern) {
    this.subs.add(
      this._reqIntern.reopenRequisition(entity.id).subscribe((rs) => {
        if (!rs.loading && rs.success) {
          this.refresh();
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.REOPEN_SUCCESS);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  onAddCandidate(entity: RequisitionIntern) {
    this.subs.add(
      this._reqIntern.getCVIdsByReqquestId(entity.id).subscribe((rs) => {
        if (!rs.loading && rs.success) {
          const seletedIds = rs.result;
          this.handleSelectedCandidateInternList(seletedIds);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  public isInprogressStatus(status: string) {
    return status === StatusEnum.InProgress;
  }

  public getListItem(isInprogress: boolean): MenuItem[] {
    return [{
      label: 'Action',
      items: [{
        label: 'Add CV',
        icon: 'pi pi-user-plus',
        command: () => {
          this.onAddCandidate(this.currentReqIntern);
        },
        visible: isInprogress && this.permission.isGranted(this.PS.Pages_RequisitionIntern_AddCV)
      }, {
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => {
          this.openDialog(this.DIALOG_ACTION.UPDATE);
        },
        visible: isInprogress && this.permission.isGranted(this.PS.Pages_RequisitionIntern_Edit)
      }, {
        label: 'Clone',
        icon: 'pi pi-clone',
        command: () => {
          this.openDialog(this.DIALOG_ACTION.CLONE);
        },
        visible: this.permission.isGranted(this.PS.Pages_RequisitionIntern_Clone)
      }, {
        label: 'Close',
        icon: 'pi pi-times',
        command: () => {
          this.onClose(this.currentReqIntern);
        },
        visible: isInprogress && this.permission.isGranted(this.PS.Pages_RequisitionIntern_Close)
      }, {
        label: 'Reopen',
        icon: 'pi pi-undo',
        command: () => {
          this.onReopen(this.currentReqIntern);
        },
        visible: !isInprogress && this.permission.isGranted(this.PS.Pages_RequisitionIntern_ReOpen)
      }, {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => {
          this.delete(this.currentReqIntern);
        },
        visible: this.permission.isGranted(this.PS.Pages_RequisitionIntern_Delete)
      }, {
        label: 'Close & Clone',
        icon: 'pi pi-copy',
        command: () => {
          this.onCloseAndClone(this.currentReqIntern);
        },
        visible: isInprogress && this.permission.isGranted(this.PS.Pages_RequisitionIntern_CloseAndClone)
      },]
    }]
  }

  private handleSelectedCandidateInternList(selectedIds: number[]) {
    const dialogRef = this._dialog.open(CandidateInternListComponent, {
      header: `Add candidate to requistion intern`,
      width: "90%",
      contentStyle: { "max-height": "100%", overflow: "auto" },
      baseZIndex: 10000,
      data: {
        dialogMode: true,
        selectedIds,
        reqInternId: this.currentReqIntern.id,
      },
    });

    dialogRef.onClose.subscribe((requisition: RequisitionIntern) => {
      this.currentReqIntern = this._reqIntern.getCurrentReqIntern();
      if (this.currentReqIntern) {
        this.updateNewRequisiton(this.currentReqIntern);
        this.getCandidateRowExpand(this.currentReqIntern);
      }
    });
  }

  onUpdatedTimeChange(talentDateTime: TalentDateTime) {
    this.searchWithUpdatedTime = talentDateTime;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  private getPayLoad(request: PagedRequestDto): RequisitionPayloadList {
    const payload: any = { ...request }
    const filterItems: Filter[] = [];

    payload.isAndCondition = this.isSearchAnd;
    this.searchWithSkills?.length && (payload.skillIds = this.searchWithSkills);

    if (checkNumber(this.searchWithReqStatus)) {
      const filterObj: Filter = { propertyName: 'status', value: this.searchWithReqStatus, comparision: COMPARISION_OPERATOR.Equal }
      filterItems.push(filterObj);
    }

    if (checkNumber(this.searchWithBranch)) {
      const filterObj: Filter = { propertyName: 'branchId', value: this.searchWithBranch, comparision: COMPARISION_OPERATOR.Equal }
      filterItems.push(filterObj);
    }

    if (checkNumber(this.searchWithSubPosition)) {
      const filterObj: Filter = { propertyName: 'subPositionId', value: this.searchWithSubPosition, comparision: COMPARISION_OPERATOR.Equal }
      filterItems.push(filterObj);
    }

    const updatedTime: TalentDateTime = this.searchWithUpdatedTime;
    if (updatedTime && updatedTime.dateType !== CreationTimeEnum.ALL) {
      const fromfilterObj: Filter = {
        propertyName: "lastModifiedTime",
        value: updatedTime?.fromDate.format(DateFormat.YYYY_MM_DD),
        comparision: COMPARISION_OPERATOR.GreaterThanOrEqual,
      };

      const tofilterObj: Filter = {
        propertyName: "lastModifiedTime",
        value: updatedTime?.toDate.format(DateFormat.YYYY_MM_DD),
        comparision: COMPARISION_OPERATOR.LessThanOrEqual,
      };

      filterItems.push(fromfilterObj);
      filterItems.push(tofilterObj);
    }

    payload.filterItems = filterItems;
    return payload;
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Requisition", routerLink: DefaultRoute.Requisition, styleClass: 'menu-item-click' }, { label: "Intern" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

  private resetRowExpand() {
    this.isRowExpand = false;
    this.expandedRows = [];
  }

  private updateNewRequisiton(reqIntern: RequisitionIntern) {
    const idx = this.reqInterns.findIndex(req => req.id === this.currentReqIntern.id)
    this.reqInterns[idx] = reqIntern;
  }
}
