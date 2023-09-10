import { Component, Injector, OnInit, Optional } from '@angular/core';
import { checkNumber } from '@app/core/helpers/utils.helper';
import { CandidateInfo } from '@app/core/models/candidate/candidate.model';
import { RequisitionPayloadList, RequisitionStaff, RequisitionStaffConfigDiaLog, RequisitonCandidate } from '@app/core/models/requisition/requisition.model';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { DateFormat, MESSAGE } from '@shared/AppConsts';
import { ActionEnum, API_RESPONSE_STATUS, COMPARISION_OPERATOR, DefaultRoute, SearchType, SortType, StatusEnum, ToastMessageType, UserType } from '@shared/AppEnums';
import { ApiResponse, Filter, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { MenuItem } from 'primeng/api';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { RequisitionStaffService } from './../../../core/services/requisition/requisition-staff.service';
import { CandidateStaffListComponent } from './../../candidate/candidate-staff-list/candidate-staff-list.component';
import { RequisitionStaffDialogComponent } from './requisition-staff-dialog/requisition-staff-dialog.component';

@Component({
  selector: 'talent-requisition-staff',
  templateUrl: './requisition-staff.component.html',
  styleUrls: ['./requisition-staff.component.scss']
})
export class RequisitionStaffComponent extends PagedListingComponentBase<RequisitionStaff> implements OnInit {
  public readonly SORT_TYPE = SortType;
  public readonly DATE_FORMAT = DateFormat;

  searchWithProcessCvStatus: number;
  reqStaffs: RequisitionStaff[] = [];
  totalQuantity: number;
  currentReqStaff: RequisitionStaff = null;
  searchWithBranch: number;
  searchWithSubPosition: number;
  searchWithSkills: number[];
  searchWithReqStatus: number = 0;
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
    private _reqStaff: RequisitionStaffService,
    private _dialog: DialogService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
    this.isDialogMode = !!this.config?.data?.dialogMode;
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload = this.getPayLoad(request);
    this.subs.add(
      this._reqStaff.getAllPagging(payload).subscribe((rs) => {
        this.reqStaffs = [];
        this.resetRowExpand();
        if (rs.success) {
          this.totalQuantity = rs.result.totalQuantity;
          this.reqStaffs = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(entity: RequisitionStaff): void {
    const deleteRequest = this._reqStaff.delete(entity.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, `#${entity.id}`).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  onCandidateSelectedRequisiton(entity: RequisitionStaff) {
    this.ref.close(entity)
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

  openDialog(dialogAction: ActionEnum) {
    const dialogConfig: RequisitionStaffConfigDiaLog = { requisitionStaff: this.currentReqStaff, action: dialogAction };
    const dialogRef = this._dialog.open(RequisitionStaffDialogComponent, {
      header: `${dialogAction} Requisition`,
      width: "55%",
      contentStyle: { overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<RequisitionStaff>) => {
      if (res) {
        if (dialogConfig.action === ActionEnum.UPDATE) {
          const index = this.reqStaffs.findIndex((x) => x.id == res.result.id);
          this.reqStaffs[index] = res.result;
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);
          this.onGetDataChange();
          return;
        }
        this.reqStaffs.unshift(res.result);
        this.showToastMessage(
          ToastMessageType.SUCCESS,
          dialogConfig.action === ActionEnum.CREATE ? MESSAGE.CREATE_SUCCESS : MESSAGE.CLONE_SUCCESS
        );
      }
    });
  }

  onGetDataChange() {
    this.resetRowExpand();
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  getCandidateRowExpand(entity: RequisitionStaff) {
    this.subs.add(
      this._reqStaff.getCVsByRequestId(entity.id).subscribe((rs) => {
        if (!rs.loading && rs.success) {
          const index = this.reqStaffs.findIndex(req => req.id === entity.id);
          if (this.searchWithProcessCvStatus) {
            this.reqStaffs[index].reqCvs = rs.result.filter(s => s.processCVStatus == this.searchWithProcessCvStatus) as RequisitonCandidate[];
          }
          else {
            this.reqStaffs[index].reqCvs = rs.result;
          }
        }
        this.isLoading = rs.loading;
      })
    );
  }

  onSkillChange(event) {
    this.searchWithSkills = event;
    this.onGetDataChange();
  }

  onSearchTypeChange(value: string) {
    this.isSearchAnd = value === SearchType.AND;
    this.onGetDataChange();
  }

  deleteRequestCv(entity: RequisitonCandidate, requisitionStaff: RequisitionStaff) {
    this.currentReqStaff = requisitionStaff;
    this.subs.add(
      this._reqStaff.deleteRequestCV(entity.id, requisitionStaff.id).subscribe((res) => {
        this.isLoading = res.loading;
        if (!res.loading && res.success) {
          this.updateNewRequisiton(res.result);
          this.getCandidateRowExpand(this.currentReqStaff);
          this.showToastMessage(ToastMessageType.SUCCESS, `Delete ${entity.fullName} successfully`);
        }
      })
    );
  }

  onClose(entity: RequisitionStaff) {
    this.subs.add(
      this._reqStaff.closeRequisition(entity.id).subscribe((rs) => {
        if (!rs.loading && rs.success) {
          this.refresh();
          this.showToastMessage(ToastMessageType.WARN, MESSAGE.CLOSE_SUCCESS);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  onReopen(entity: RequisitionStaff) {
    this.subs.add(
      this._reqStaff.reopenRequisition(entity.id).subscribe((rs) => {
        if (!rs.loading && rs.success) {
          this.refresh();
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.REOPEN_SUCCESS);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  onAddCandidate(entity: RequisitionStaff) {
    this.subs.add(
      this._reqStaff.getCVIdsByReqquestId(entity.id).subscribe((rs) => {
        if (!rs.loading && rs.success) {
          const seletedIds = rs.result;
          this.handleSelectedCandidateStaffList(seletedIds);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  public isInprogressStatus(status: string) {
    return status === StatusEnum.InProgress;
  }

  public getListItem(isInprogress: boolean, isProjectTool: boolean): MenuItem[] {
    return [{
      label: 'Action',
      items: [{
        label: 'Add CV',
        icon: 'pi pi-user-plus',
        command: () => {
          this.onAddCandidate(this.currentReqStaff);
        },
        visible: isInprogress && this.permission.isGranted(this.PS.Pages_RequisitionStaff_AddCV)
      }, {
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => {
          this.openDialog(this.DIALOG_ACTION.UPDATE);
        },
        visible: isInprogress && this.permission.isGranted(this.PS.Pages_RequisitionStaff_Edit)
      }, {
        label: 'Clone',
        icon: 'pi pi-clone',
        command: () => {
          this.openDialog(this.DIALOG_ACTION.CLONE);
        },
        visible: this.permission.isGranted(this.PS.Pages_RequisitionStaff_Clone)
      }, {
        label: 'Close',
        icon: 'pi pi-times',
        command: () => {
          this.onClose(this.currentReqStaff);
        },
        visible: isInprogress /*&& !isProjectTool*/ && this.permission.isGranted(this.PS.Pages_RequisitionStaff_Close)
      }, {
        label: 'Reopen',
        icon: 'pi pi-undo',
        command: () => {
          this.onReopen(this.currentReqStaff);
        },
        visible: !isInprogress && this.permission.isGranted(this.PS.Pages_RequisitionStaff_ReOpen)
      }, {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => {
          this.delete(this.currentReqStaff);
        },
        visible: !isProjectTool && this.permission.isGranted(this.PS.Pages_RequisitionStaff_Delete)
      }]
    }]
  }

  private handleSelectedCandidateStaffList(selectedIds: number[]) {
    const dialogRef = this._dialog.open(CandidateStaffListComponent, {
      header: `Add candidate to requistion staff`,
      width: "90%",
      contentStyle: { "max-height": "100%", overflow: "auto" },
      baseZIndex: 10000,
      data: { dialogMode: true, selectedIds, reqStaffId: this.currentReqStaff.id, },
    });

    dialogRef.onClose.subscribe((requisition: RequisitionStaff) => {
      this.currentReqStaff = this._reqStaff.getCurrentReqStaff();
      if (this.currentReqStaff) {
        this.updateNewRequisiton(this.currentReqStaff);
        this.getCandidateRowExpand(this.currentReqStaff);
      }
    });
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

    payload.filterItems = filterItems;
    return payload;
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Requisition", routerLink: DefaultRoute.Requisition, styleClass: 'menu-item-click' }, { label: "Staff" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

  private resetRowExpand() {
    this.isRowExpand = false;
    this.expandedRows = [];
  }

  private updateNewRequisiton(reqStaff: RequisitionStaff) {
    const idx = this.reqStaffs.findIndex(req => req.id === this.currentReqStaff.id);
    this.reqStaffs[idx] = reqStaff;
  }
}
