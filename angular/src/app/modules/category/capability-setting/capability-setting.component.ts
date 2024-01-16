import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { CapabilitySetting, CapabilitySettingConfigDiaLog } from '@app/core/models/categories/capabilities-setting.model';
import { BreadCrumbConfig } from '@app/core/models/common/common.dto';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { ActionEnum, API_RESPONSE_STATUS, ToastMessageType } from '@shared/AppEnums';
import { ConfirmationService } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DefaultRoute } from './../../../../shared/AppEnums';
import { CapabilitySettingService } from './../../../core/services/categories/capability-setting.service';
import { EditGuideLineDialogComponent } from './edit-guideline-dialog/edit-guideline-dialog.component';
import { MESSAGE } from '@shared/AppConsts';
import {
  ApiResponse,
  PagedListingComponentBase,
  PagedRequestDto
} from "@shared/paged-listing-component-base";
import { CapabilityWithSetting } from '@app/core/models/categories/capabilities-setting.model';
import { DataSharingService } from '@app/core/services/categories/data-sharing-capabillity.service';

@Component({
  selector: 'talent-capability-setting',
  templateUrl: './capability-setting.component.html',
  styleUrls: ['./capability-setting.component.scss']
})
export class CapabilitySettingComponent extends PagedListingComponentBase<CapabilitySetting> implements OnInit, OnDestroy {

  readonly GET_FIRST_PAGE = 1;
  catCapabilities = [
    { fromType : false, name: 'Reviewer'},
    { fromType : true, name: 'HR'}];
  searchWithFrom: boolean = null;
  searchWithUserType: number = null;
  searchWithSubPosition: number = null;
  searchCapabilityName: string = null;
  searchFactor: number = null;
  capSettings: CapabilitySetting[] = [];
  breadcrumbConfig: BreadCrumbConfig;
  first = 0;

  private dialogRef: DynamicDialogRef;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    public _utilities: UtilitiesService,
    private _capabilitySetting: CapabilitySettingService,
    private _confirmation: ConfirmationService,
    private dataSharingService: DataSharingService
  ) {
    super(injector);
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subs.add(
      this._capabilitySetting.getAllPagging(request).subscribe((rs) => {
        this.capSettings = [];
        if (rs.success) {
          this.capSettings = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
    this.getAll(this.GET_FIRST_PAGE);
  }

  getRowspanCapSetting(item: CapabilitySetting) {
    if (!item.capabilities || item.capabilities.length === 0) {
      return 1;
    }
    return item.capabilities.length;
  }

  openDialog(capWithSetting: CapabilityWithSetting, capSetting: CapabilitySetting, dialogAction: ActionEnum) {
    const dialogConfig: CapabilitySettingConfigDiaLog = { capabilitySetting: capSetting, capabilityWithSetting: capWithSetting , action: dialogAction };
    const dialogRef = this.dialogService.open(EditGuideLineDialogComponent, {
      header: `${dialogConfig.action} GuideLine`,
      width: "60%",
      contentStyle: {  overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });
    dialogRef.onClose.subscribe((res: ApiResponse<CapabilityWithSetting>) => {
      if(!res) return; 
      if (dialogConfig.action === ActionEnum.UPDATE) {
        this.dataSharingService.guideLine$.subscribe(payload => {
          if(capWithSetting.id === payload?.id && capWithSetting.capabilityId === payload.capabilityId){
            capWithSetting.guideLine= payload.guideLine
          }
        })
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.guideLine);
        return;
      }
      this.refresh();
    });
  }

  ngOnDestroy() {
    if (this.dialogRef) this.dialogRef.close()
  }

  createDialog() {
    this.router.navigate(['capabilities'], {
      relativeTo: this.route,
      queryParams: { subPositionId: '', userType: '' }
    });
  }

  onEditTable(item: CapabilitySetting) {
    this.router.navigate(['capabilities'], {
      relativeTo: this.route,
      queryParams: { subPositionId: item.subPositionId, userType: item.userType }
    });
  }

   onPositionSelect(subPositionId: number) {
    this.searchWithSubPosition = subPositionId;
    this.getAll(this.GET_FIRST_PAGE);
  }

  delete(capability: CapabilitySetting): void {
    const confirmName = `Setting ${capability.userTypeName} - ${capability.subPositionName}`;

    const deleteRequest = this._capabilitySetting.deleteCapabilitySetting(capability.userType, capability.subPositionId);
    this.deleteConfirmAndShowToastMessage(deleteRequest, confirmName).subscribe((message) => {
      if (message === API_RESPONSE_STATUS.SUCCESS) {
        this.getAll();
      }
    })
  }

  getAll(pageNumber?: number) {
    this.first = pageNumber === 1 ? 0 : this.first;
    const payload = {
      capabilityName: this.searchCapabilityName,
      userType: this.searchWithUserType,
      subPositionId: this.searchWithSubPosition,
      factor: this.searchFactor,
      fromType: this.searchWithFrom
    };
    this.subs.add(
        this._capabilitySetting.getAllCapabilitiesSettings(payload)
            .subscribe((rs) => rs.success && (this.capSettings = rs.result))
        );
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Categories", routerLink: DefaultRoute.Category, styleClass: 'menu-item-click' }, { label: "Capability Settings" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}
