import { Component, Injector, OnDestroy, OnInit } from "@angular/core";
import { EmployeePositionService } from './../../../core/services/employee-profile/employee-position.service';

import {
  ActionEnum,
  API_RESPONSE_STATUS,
  DefaultRoute,
  ToastMessageType
} from "@shared/AppEnums";
import {
  ApiResponse,
  PagedListingComponentBase,
  PagedRequestDto
} from "@shared/paged-listing-component-base";
import { DialogService, DynamicDialogRef } from "primeng/dynamicdialog";
import { EmployeePosition,EmployeePositionConfigDiaLog } from './../../../core/models/employee-profile/employee-position-model';
import { EmployeePositionDialogComponent } from "./employee-position-dialog/employee-position-dialog.component"; 
import { MESSAGE } from "@shared/AppConsts";

@Component({
  selector: 'talent-employee-position',
  templateUrl: './employee-position.component.html',
  styleUrls: ['./employee-position.component.scss']
})
export class EmployeePositionComponent extends PagedListingComponentBase<EmployeePosition> implements OnInit, OnDestroy {
  public employeePositions: EmployeePosition[] = [];
  dialogRef: DynamicDialogRef;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _employeePositionService: EmployeePositionService
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

  openDialog(obj: EmployeePosition, dialogAction: ActionEnum) {
    const dialogConfig: EmployeePositionConfigDiaLog = { employeePosition: obj, action: dialogAction }
    this.dialogRef = this.dialogService.open(EmployeePositionDialogComponent, {
      header: `${dialogConfig.action} Employee Position`,
      width: "40%",
      contentStyle: { "max-height": "500px", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    this.dialogRef.onClose.subscribe((res: ApiResponse<EmployeePosition>) => {
      if (dialogConfig.action === ActionEnum.UPDATE) {
        const index = this.employeePositions.findIndex((x) => x.id == res.result.id);
        this.employeePositions[index] = res.result;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.name);
        return;
      }
      this.refresh();
    });
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subs.add(
      this._employeePositionService.getAllPagging(request).subscribe((rs) => {
        this.employeePositions = [];
        if (rs.success) {
          this.employeePositions = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(entity: EmployeePosition): void {
    const deleteRequest = this._employeePositionService.delete(entity.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, entity.name).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Employee Profile", routerLink: DefaultRoute.Employee_Profile, styleClass: 'menu-item-click' }, { label: "Employee Position" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}
