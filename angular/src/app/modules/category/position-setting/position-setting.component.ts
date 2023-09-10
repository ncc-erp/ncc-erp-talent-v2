import { Component, Injector, OnInit } from '@angular/core';
import { checkNumber } from '@app/core/helpers/utils.helper';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { MESSAGE } from '@shared/AppConsts';
import { API_RESPONSE_STATUS, COMPARISION_OPERATOR, DefaultRoute, ToastMessageType } from '@shared/AppEnums';
import { ApiResponse, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { DialogService } from 'primeng/dynamicdialog';
import { PositionSetting, PositionSettingUpdate } from './../../../core/models/categories/position-setting.model';
import { PositionSettingService } from './../../../core/services/categories/position-setting.service';
import { LmsSettingDialogComponent } from './lms-setting-dialog/lms-setting-dialog.component';
import { PositionSettingDialogComponent } from './position-setting-dialog/position-setting-dialog.component';

@Component({
  selector: 'talent-position-setting',
  templateUrl: './position-setting.component.html',
  styleUrls: ['./position-setting.component.scss']
})
export class PositionSettingComponent extends PagedListingComponentBase<PositionSetting> implements OnInit {

  positionSettings: PositionSetting[] = [];
  searchDetail = {
    subPositionId: null,
    userType: null
  }
  isConnectionToLMSFailed: boolean = false;
  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    private _positionSetting: PositionSettingService,
    private _dialog: DialogService

  ) {
    super(injector);
    this.breadcrumbConfig = this.getBreadcrumbConfig();
  }

  ngOnInit() {
    this.isConnectionToLMSFailed = !(this._utilities.catLmsCourse && this._utilities.catLmsCourse?.length);
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload = this.getPayload(request);
    this.subs.add(
      this._positionSetting.getAllPagging(payload).subscribe((rs) => {
        this.positionSettings = [];
        if (rs.success) {
          this.positionSettings = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }
  protected delete(entity: PositionSetting): void {
    const deleteRequest = this._positionSetting.delete(entity.id);
    const title = `${entity.userTypeName} - ${entity.subPositionName}`
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, title).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) this.refresh();
      })
    );
  }

  onPositionSelect(subPositionId: number) {
    this.searchDetail.subPositionId = subPositionId;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  onCreate() {
    const dialogRef = this._dialog.open(PositionSettingDialogComponent, {
      header: `Create position setting`,
      width: "40%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      baseZIndex: 5000,
      data: null,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<PositionSetting>) => {
      this.refresh();
    });
  }

  onUpdateLms(entity: PositionSetting) {
    const positionSettingUpdate: PositionSettingUpdate = { ...entity };
    const dialogRef = this._dialog.open(LmsSettingDialogComponent, {
      header: `Update LMS setting`,
      width: "40%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      baseZIndex: 5000,
      data: positionSettingUpdate,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<PositionSetting>) => {
      if (res) {
        const index = this.positionSettings.findIndex((x) => x.id == res.result.id);
        this.positionSettings[index] = res.result;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.lmsCourseName);
        return;
      }
    });
  }

  private getPayload(request: PagedRequestDto): PagedRequestDto {
    const filterItems = [];
    for (const property in this.searchDetail) {
      if (checkNumber(this.searchDetail[property])) {
        const filterObj = {
          propertyName: property,
          value: this.searchDetail[property],
          comparision: COMPARISION_OPERATOR.Equal,
        }
        filterItems.push(filterObj);
      }
    }

    request.filterItems = filterItems;
    return request;
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [
        { label: "Categories", routerLink: DefaultRoute.Category, styleClass: 'menu-item-click' },
        { label: "Position Settings" }
      ],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}
