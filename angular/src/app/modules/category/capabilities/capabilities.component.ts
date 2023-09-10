import { Component, Injector, OnDestroy, OnInit } from "@angular/core";
import { Capability, CapabilityConfigDiaLog } from '@app/core/models/categories/capabilities.model';
import { MESSAGE } from '@shared/AppConsts';
import {
  ActionEnum,
  API_RESPONSE_STATUS, COMPARISION_OPERATOR, DefaultRoute, ToastMessageType
} from "@shared/AppEnums";
import {
  ApiResponse,
  Filter,
  PagedListingComponentBase,
  PagedRequestDto
} from "@shared/paged-listing-component-base";
import { DialogService, DynamicDialogRef } from "primeng/dynamicdialog";
import { CapabilityService } from './../../../core/services/categories/capability.service';
import { CapabilityDialogComponent } from './capability-dialog/capability-dialog.component';

@Component({
  selector: 'talent-capabilities',
  templateUrl: './capabilities.component.html',
  styleUrls: ['./capabilities.component.scss']
})
export class CapabilitiesComponent extends PagedListingComponentBase<Capability> implements OnInit, OnDestroy {

  catCapabilities = [
    { fromType : false, name: 'Reviewer'},
    { fromType : true, name: 'HR'}];
  searchWithFrom: boolean = null;

  capabilities: Capability[] = [];

  private dialogRef: DynamicDialogRef;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _capability: CapabilityService
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

  openDialog(obj: Capability, dialogAction: ActionEnum) {
    const dialogConfig: CapabilityConfigDiaLog = { capability: obj, action: dialogAction };
    const dialogRef = this.dialogService.open(CapabilityDialogComponent, {
      header: `${dialogConfig.action} Capability`,
      width: "60%",
      contentStyle: {  overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<Capability>) => {
      if (dialogConfig.action === ActionEnum.UPDATE && res) {
        const index = this.capabilities.findIndex((x) => x.id == res.result.id);
        this.capabilities[index] = res.result;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.name);
        return;
      }
      this.refresh();
    });
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload = { ...request};
    const filterItems : Filter[] = [];
    if( typeof this.searchWithFrom === 'boolean' ){
      const objFil = { propertyName: 'fromType', value: this.searchWithFrom, comparision: COMPARISION_OPERATOR.Equal }
      filterItems.push(objFil);
    }
    payload.filterItems = filterItems;
    this.subs.add(
      this._capability.getAllPagging(payload).subscribe((rs) => {
        this.capabilities = [];
        if (rs.success) {
          this.capabilities = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(capability: Capability): void {
    const deleteRequest = this._capability.delete(capability.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, capability.name).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Categories", routerLink: DefaultRoute.Category, styleClass: 'menu-item-click' }, { label: "Capabilities" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

}
