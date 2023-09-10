import { Component, Injector, OnDestroy, OnInit } from "@angular/core";
import { Branch, BranchConfigDiaLog } from '@app/core/models/categories/branch.model';
import { MESSAGE } from '@shared/AppConsts';
import {
  ActionEnum,
  API_RESPONSE_STATUS, DefaultRoute, ToastMessageType
} from "@shared/AppEnums";
import {
  ApiResponse,
  PagedListingComponentBase,
  PagedRequestDto
} from "@shared/paged-listing-component-base";
import { DialogService, DynamicDialogRef } from "primeng/dynamicdialog";
import { BranchService } from './../../../core/services/categories/branch.service';
import { BranchDialogComponent } from './branch-dialog/branch-dialog.component';

@Component({
  selector: 'talent-branch',
  templateUrl: './branch.component.html',
  styleUrls: ['./branch.component.scss']
})
export class BranchComponent extends PagedListingComponentBase<Branch> implements OnInit, OnDestroy {

  public branches: Branch[] = [];
  private dialogRef: DynamicDialogRef;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _branch: BranchService
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

  openDialog(obj: Branch, dialogAction: ActionEnum) {
    const dialogConfig: BranchConfigDiaLog = { branch: obj, action: dialogAction };
    const dialogRef = this.dialogService.open(BranchDialogComponent, {
      header: `${dialogConfig.action} Branch`,
      width: "40%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<Branch>) => {
      if (dialogConfig.action === ActionEnum.UPDATE) {
        const index = this.branches.findIndex((x) => x.id == res.result.id);
        this.branches[index] = res.result;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.displayName);
        return;
      }
      this.refresh();
    });
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subs.add(
      this._branch.getAllPagging(request).subscribe((rs) => {
        this.branches = [];
        if (rs.success) {
          this.branches = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(branch: Branch): void {
    const deleteRequest = this._branch.delete(branch.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, branch.displayName).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Categories", routerLink: DefaultRoute.Category, styleClass: 'menu-item-click' }, { label: "Branch" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}
