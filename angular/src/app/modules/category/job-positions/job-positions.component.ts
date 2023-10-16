import { DefaultRoute } from './../../../../shared/AppEnums';
import { Component, Injector, OnDestroy, OnInit } from "@angular/core";
import { JobPosition, JobPositionConfigDiaLog } from '@app/core/models/categories/job-position.model';
import {
  ActionEnum,
  API_RESPONSE_STATUS, ToastMessageType
} from "@shared/AppEnums";
import {
  ApiResponse,
  PagedListingComponentBase,
  PagedRequestDto
} from "@shared/paged-listing-component-base";
import { DialogService, DynamicDialogRef } from "primeng/dynamicdialog";
import { JobPositionService } from './../../../core/services/categories/job-position.service';
import { JobPositionDialogComponent } from './job-position-dialog/job-position-dialog.component';
import { MESSAGE } from '@shared/AppConsts';

@Component({
  selector: 'talent-job-positions',
  templateUrl: './job-positions.component.html',
  styleUrls: ['./job-positions.component.scss']
})
export class JobPositionsComponent extends PagedListingComponentBase<JobPosition> implements OnInit, OnDestroy {
  public jobPositions: JobPosition[] = [];
  private dialogRef: DynamicDialogRef;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _jobPosition: JobPositionService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
  }

  ngOnDestroy() {
    super.ngOnDestroy();
    if (this.dialogRef) this.dialogRef.close()
  }

  openDialog(obj: JobPosition, dialogAction: ActionEnum) {
    const dialogConfig: JobPositionConfigDiaLog = { jobPosition: obj, action: dialogAction,showButtonSave:true };
    const dialogRef = this.dialogService.open(JobPositionDialogComponent, {
      header: `${dialogConfig.action} Job Position`,
      width: "40%",
      contentStyle: { "max-height": "500px", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<JobPosition>) => {
      if (dialogConfig.action === ActionEnum.UPDATE && res) {
        const index = this.jobPositions.findIndex((x) => x.id == res.result.id);
        this.jobPositions[index] = res.result;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.name);
        return;
      }
      this.refresh();
    });
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subs.add(
      this._jobPosition.getAllPagging(request).subscribe((rs) => {
        this.jobPositions = [];
        if (rs.success) {
          this.jobPositions = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(position: JobPosition): void {
    const deleteRequest = this._jobPosition.delete(position.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, position.name).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Categories", routerLink: DefaultRoute.Category, styleClass: 'menu-item-click' }, { label: "Job Positions" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

}
