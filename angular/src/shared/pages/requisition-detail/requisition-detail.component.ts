import { Component, Injector, OnInit, Optional } from '@angular/core';
import { CandidateInfo } from '@app/core/models/candidate/candidate.model';
import { RequisitionCloseAndClone, RequisitionInternConfigDiaLog, RequisitionStaff } from '@app/core/models/requisition/requisition.model';
import { RequisitionInternService } from '@app/core/services/requisition/requisition-intern.service';
import { RequisitionStaffService } from '@app/core/services/requisition/requisition-staff.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { CandidateInternListComponent } from '@app/modules/candidate/candidate-intern-list/candidate-intern-list.component';
import { CandidateStaffListComponent } from '@app/modules/candidate/candidate-staff-list/candidate-staff-list.component';
import { RequisitionInternDialogComponent } from '@app/modules/requisitiion/requisition-intern/requisition-intern-dialog/requisition-intern-dialog.component';
import { RequisitionStaffDialogComponent } from '@app/modules/requisitiion/requisition-staff/requisition-staff-dialog/requisition-staff-dialog.component';
import { AppComponentBase } from '@shared/app-component-base';
import { DateFormat, MESSAGE } from '@shared/AppConsts';
import { ActionEnum, API_RESPONSE_STATUS, StatusEnum, ToastMessageType, UserType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { MenuItem } from 'primeng/api';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { last, mergeMap } from 'rxjs/operators';
import { RequisitionIntern, RequisitionStaffConfigDiaLog, RequisitonCandidate } from './../../../app/core/models/requisition/requisition.model';

@Component({
  selector: 'talent-requisition-detail',
  templateUrl: './requisition-detail.component.html',
  styleUrls: ['./requisition-detail.component.scss']
})
export class RequisitionDetailComponent extends AppComponentBase implements OnInit {
  public readonly DATE_FORMAT = DateFormat;

  btnBreadCrumbConfig = {
    label: 'Back',
    icon: 'pi pi-chevron-left'
  }

  requisitions: RequisitionStaff[] | RequisitionIntern[] = [];
  currentRequisition: RequisitionStaff | RequisitionIntern = null;
  expandedRows: { [s: number]: boolean } = {};

  reqType: number;
  isRequestIntern: boolean;
  _requisition: RequisitionInternService | RequisitionStaffService;
  requisitionId: number;

  constructor(
    injector: Injector,
    @Optional() public config: DynamicDialogConfig,
    @Optional() public ref: DynamicDialogRef,
    public _utilities: UtilitiesService,
    private _reqStaff: RequisitionStaffService,
    private _reqIntern: RequisitionInternService,
    private _dialog: DialogService
  ) {
    super(injector);

    this.route.params.subscribe(params => {
      this.requisitionId = params.id;
      this.reqType = Number(this.route.snapshot.queryParams?.type);
      this.isRequestIntern = this.reqType === UserType.INTERN;
      this._requisition = this.isRequestIntern ? this._reqIntern : this._reqStaff;

      this.breadcrumbConfig = this.getBreadcrumbConfig();
      this.getRequisitionDetail();
    })

  }

  ngOnInit(): void { }

  onCandidateSelectedRequisiton(entity: RequisitionStaff) {
    this.ref.close(entity)
  }

  navigateToRequestDetail(id: number) {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { type: this.reqType },
      queryParamsHandling: 'merge',
    });
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
    if (this.isRequestIntern) {
      this._openRequestInternDialog(dialogAction, reqCloseAndClone);
      return;
    }
    this._openRequestStaffDialog(dialogAction);
  }

  goBack() {
    const path = this.isRequestIntern ? 'req-intern' : 'req-staff';
    this.router.navigate(["/app/requisition/", path]);
  }

  onClose(entity: RequisitionStaff | RequisitionIntern) {
    this.subs.add(
      this._requisition.closeRequisition(entity.id).subscribe((rs) => {
        this.isLoading = rs.loading;
        if (!rs.loading && rs.success) {
          this.getRequisitionDetail();
          this.showToastMessage(ToastMessageType.WARN, MESSAGE.CLOSE_SUCCESS);
        }
      })
    );
  }

  onReopen(entity: RequisitionStaff | RequisitionIntern) {
    this.subs.add(
      this._requisition.reopenRequisition(entity.id).subscribe((rs) => {
        if (!rs.loading && rs.success) {
          this.getRequisitionDetail();
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.REOPEN_SUCCESS);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  onAddCandidate(entity: RequisitionStaff) {
    this.subs.add(
      this._requisition.getCVIdsByReqquestId(entity.id).subscribe((rs) => {
        if (!rs.loading && rs.success) {
          const seletedIds = rs.result;
          this.handleSelectedCandidateList(seletedIds);
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

  getCandidateRowExpand() {
    this.subs.add(
      this._requisition.getCVsByRequestId(this.requisitionId).subscribe((rs) => {
        if (!rs.loading && rs.success) {
          this.requisitions[0].reqCvs = rs.result;
        }
        this.isLoading = rs.loading;
      })
    );
  }

  delete(entity: RequisitionStaff | RequisitionIntern): void {
    const deleteRequest = this._requisition.delete(entity.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, `#${entity.id}`).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.getRequisitionDetail();
        }
      })
    );
  }

  deleteRequestCv(entity: RequisitonCandidate, requisition: RequisitionStaff | RequisitionIntern) {
    this.currentRequisition = requisition;
    this.subs.add(
      this._requisition.deleteRequestCV(entity.id, requisition.id).subscribe((res) => {
        this.isLoading = res.loading;
        if (!res.loading && res.success) {
          this.updateNewRequisiton(res.result);
          this.getCandidateRowExpand();
          this.showToastMessage(ToastMessageType.SUCCESS, `Delete ${entity.fullName} successfully`);
        }
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
          this.onAddCandidate(this.currentRequisition);
        },
        visible: isInprogress && this.validPermissionUserType(this.reqType, this.PS.Pages_RequisitionIntern_AddCV, this.PS.Pages_RequisitionStaff_AddCV)
      }, {
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => {
          this.openDialog(this.DIALOG_ACTION.UPDATE);
        },
        visible: isInprogress && this.validPermissionUserType(this.reqType, this.PS.Pages_RequisitionIntern_Edit, this.PS.Pages_RequisitionStaff_Edit)
      }, {
        label: 'Clone',
        icon: 'pi pi-clone',
        command: () => {
          this.openDialog(this.DIALOG_ACTION.CLONE);
        },
        visible: this.validPermissionUserType(this.reqType, this.PS.Pages_RequisitionIntern_Clone, this.PS.Pages_RequisitionStaff_Clone)
      }, {
        label: 'Close',
        icon: 'pi pi-times',
        command: () => {
          this.onClose(this.currentRequisition);
        },
        visible: isInprogress /*&& this.handleShowHideAction(isProjectTool)*/ && this.validPermissionUserType(this.reqType, this.PS.Pages_RequisitionIntern_Close, this.PS.Pages_RequisitionStaff_Close)
      }, {
        label: 'Reopen',
        icon: 'pi pi-undo',
        command: () => {
          this.onReopen(this.currentRequisition);
        },
        visible: !isInprogress && this.validPermissionUserType(this.reqType, this.PS.Pages_RequisitionIntern_ReOpen, this.PS.Pages_RequisitionStaff_ReOpen)
      }, {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => {
          this.delete(this.currentRequisition);
        },
        visible: this.handleShowHideAction(isProjectTool) && this.validPermissionUserType(this.reqType, this.PS.Pages_RequisitionIntern_Delete, this.PS.Pages_RequisitionStaff_Delete)
      }, {
        label: 'Close & Clone',
        icon: 'pi pi-copy',
        command: () => {
          this.onCloseAndClone(this.currentRequisition);
        },
        visible: this.isRequestIntern && isInprogress && this.isGranted(this.PS.Pages_RequisitionIntern_CloseAndClone)
      }]
    }]
  }

  private handleShowHideAction(isProjectTool: boolean) {
    if (this.isRequestIntern) return true;
    if (!this.isRequestIntern && !isProjectTool) return true;
    return false;
  }

  private navigateToNewRequest(requisitionId: number) {
    const reqPath = this.isRequestIntern ? 'req-intern' : 'req-staff';
    this.router.navigate([`/app/requisition/${reqPath}/`, requisitionId], {
      queryParams: { type: this.reqType },
    });
  }

  private handleSelectedCandidateList(selectedIds: number[]) {
    const candidateComponent = this.isRequestIntern ? CandidateInternListComponent : CandidateStaffListComponent;
    const dataDialogConfig = {
      dialogMode: true, selectedIds
    }
    this.isRequestIntern ? (dataDialogConfig['reqInternId'] = this.requisitionId) : (dataDialogConfig['reqStaffId'] = this.requisitionId)

    const dialogRef = this._dialog.open(candidateComponent, {
      header: `Add Candidate to requistion`,
      width: "90%",
      contentStyle: { "max-height": "100%", overflow: "auto" },
      baseZIndex: 10000,
      data: dataDialogConfig,
    });

    dialogRef.onClose.subscribe((requisition: RequisitionStaff | RequisitionIntern) => {
      this.currentRequisition = (this.isRequestIntern) ? this._reqIntern.getCurrentReqIntern() : this._reqStaff.getCurrentReqStaff();
      if (this.currentRequisition) {
        this.updateNewRequisiton(this.currentRequisition);
        this.getCandidateRowExpand();
      }
    });
  }

  private getBreadcrumbConfig() {
    const requestType = this.isRequestIntern ? 'Intern' : 'Staff';
    const path = this.isRequestIntern ? 'req-intern' : 'req-staff';
    return {
      menuItem: [
        { label: `Requisition ${requestType}`, routerLink: `/app/requisition/${path}`, styleClass: 'menu-item-click' },
        { label: `#${this.requisitionId}` }
      ],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

  private updateNewRequisiton(req: RequisitionStaff | RequisitionIntern) {
    this.requisitions = [req];
  }

  private getRequisitionDetail(): void {
    this.subs.add(
      this._requisition.getById(this.requisitionId).pipe(
        last(),
        mergeMap(reqRes => {
          this.requisitions = [];
          this.isLoading = reqRes.loading;
          if (reqRes.success && reqRes.result) {
            this.expandedRows[reqRes.result.id] = true;
            this.requisitions.push(reqRes.result);
          }
          return this._requisition.getCVsByRequestId(this.requisitionId);
        })
      ).subscribe(res => {
        this.isLoading = res.loading;
        if (!res.loading && res.success) {
          this.requisitions.length && (this.requisitions[0].reqCvs = res.result);
        }
      })
    );
  }

  private _openRequestInternDialog(dialogAction: ActionEnum, reqCloseAndClone: RequisitionCloseAndClone = null) {
    const dialogConfig: RequisitionInternConfigDiaLog = { requisitionIntern: this.currentRequisition, action: dialogAction, reqCloseAndClone };
    const dialogRef = this._dialog.open(RequisitionInternDialogComponent, {
      header: `${dialogAction} Requisition`,
      width: "55%",
      contentStyle: { "max-height": "100%", overflow: "auto" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<RequisitionIntern>) => {
      if (res) {
        this.requisitions = [res.result];
        if (dialogConfig.action === ActionEnum.UPDATE) {
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);
          this.getRequisitionDetail();
          return;
        }

        //Clone
        this.requisitionId = res.result.id;
        this.navigateToNewRequest(res.result.id);
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CLONE_SUCCESS);
      }
    });
  }

  private _openRequestStaffDialog(dialogAction: ActionEnum) {
    const dialogConfig: RequisitionStaffConfigDiaLog = { requisitionStaff: this.currentRequisition, action: dialogAction };
    const dialogRef = this._dialog.open(RequisitionStaffDialogComponent, {
      header: `${dialogAction} Requisition`,
      width: "55%",
      contentStyle: { overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<RequisitionStaff>) => {
      if (res) {
        this.requisitions = [res.result];
        if (dialogConfig.action === ActionEnum.UPDATE) {
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);
          this.getRequisitionDetail();
          return;
        }

        this.requisitionId = res.result.id;
        this.navigateToNewRequest(res.result.id);
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CLONE_SUCCESS);
      }
    });
  }
}
