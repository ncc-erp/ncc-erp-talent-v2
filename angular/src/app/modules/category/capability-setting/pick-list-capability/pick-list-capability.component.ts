import { CloneCapabilitySettingComponent } from './../clone-capability-setting/clone-capability-setting.component';
import { DialogService } from 'primeng/dynamicdialog';
import { Component, Injector, OnInit } from '@angular/core';
import { CapabilitySettingClone, CapabilitySettingPayload, CloneCapabilitySettingConfigDiaLog } from '@app/core/models/categories/capabilities-setting.model';
import { CapabilitySettingService } from '@app/core/services/categories/capability-setting.service';
import { AppComponentBase } from '@shared/app-component-base';
import { ToastMessageType } from '@shared/AppEnums';
import { forkJoin } from 'rxjs';
import { CapabilitySettingCreateResponse } from './../../../../core/models/categories/capabilities-setting.model';
import { UtilitiesService } from './../../../../core/services/utilities.service';
import { checkNumber } from '@app/core/helpers/utils.helper';
import { values,cloneDeep } from 'lodash-es';

@Component({
  selector: 'talent-pick-list-capability',
  templateUrl: './pick-list-capability.component.html',
  styleUrls: ['./pick-list-capability.component.scss']
})
export class PickListCapabilityComponent extends AppComponentBase implements OnInit {
  availableList: CapabilitySettingCreateResponse[] = [];
  selectedList: CapabilitySettingCreateResponse[] = [];
  userType: number = null;
  subPositionId: number = null;
  canEditFactor: boolean = false;
  cloneSelectedList: CapabilitySettingCreateResponse[] = [];
  cloneAvailableList: CapabilitySettingCreateResponse[] = [];

  constructor(
    private injector: Injector,
    public _utilities: UtilitiesService,
    private _capabilitySetting: CapabilitySettingService,
    private _dialog: DialogService
  ) {
    super(injector);
    this.isLoading = true;
    const { subPositionId, userType } = this.route.snapshot.queryParams;
    this.userType = userType !== '' ? +userType : null;
    this.subPositionId = subPositionId !== '' ? +subPositionId : null;
    this.getListCapabilities();
  }

  ngOnInit(): void { }

  onSourceSelect(event): void {
    if (event.items.length > 1) {
      event.items.splice(0, event.items.length - 1);
    }
  }

  onTargetSelect(event): void {
    if (event.items.length > 1) {
      event.items.splice(0, event.items.length - 1);
    }
  }

  deleteCapability(obj: CapabilitySettingCreateResponse) {
    this._capabilitySetting.delete(obj.id).subscribe(rs => {
      if (!rs.loading) {
        this.getListCapabilities();
        this.showToastMessage(rs.success ? ToastMessageType.SUCCESS : ToastMessageType.ERROR, rs.result, obj.capabilityName);
      }
    })
  }

  onUserTypeChange(userType: number) {
    this.handleSelectChange(userType, this.subPositionId);
  }

  onPositionSelect(subPositionId: number) {
    this.subPositionId = subPositionId;
    this.handleSelectChange(this.userType, subPositionId);
  }

  onEditFactor() {
    if(this.selectedList.length){
      this.canEditFactor = true;
    }
    this.cloneSelectedList = cloneDeep(this.selectedList);
    this.cloneAvailableList = cloneDeep(this.availableList);
  }
  onDoneEditFactor() {
    this.canEditFactor = false;
    this.subs.add(
      this._capabilitySetting.updateFactor(this.selectedList).subscribe(rs => {
        if (!rs.loading && rs.success) {
          this.getListCapabilities();
          this.showToastMessage(ToastMessageType.SUCCESS, 'Update Factor Successful!');
        }
      })
    )
  }
  onCancelEditFactor() {
    this.canEditFactor = false;
    this.selectedList = { ...this.cloneSelectedList };
    this.selectedList = cloneDeep(this.cloneSelectedList);
    if (this.availableList.length > 0) {
      for (let index = 0; index < this.availableList.length; index++) {
        const element = this.availableList[index].capabilityId;
        const addBack = this.cloneSelectedList.find(values => values.capabilityId == element)
        if (addBack) {
          addBack.id = 0;
          this.subs.add(
            this._capabilitySetting.create(addBack).subscribe(rs => {
              if (!rs.loading && rs.success) {
                const index = this.selectedList.findIndex(item => item.capabilityName === rs.result?.capabilityName)
                this.selectedList[index] = {
                  ...this.selectedList[index],
                  id: rs.result.id,
                  subPositionId: rs.result.subPositionId,
                  subPositionName: rs.result.subPositionName,
                  userType: rs.result.userType,
                  userTypeName: rs.result.userTypeName,
                  factor: rs.result.factor,
                  isDeleted: rs.result.isDeleted
                }
                this.getListCapabilities();
                this.showToastMessage(ToastMessageType.SUCCESS, 'Cancel Edit!!');
              }
            })
          )
        }
      }
    }
    this.availableList = cloneDeep(this.cloneAvailableList);
  }
  onClone() {
    const dialogData: CloneCapabilitySettingConfigDiaLog = { userTypeFrom: this.userType, subPositionIdFrom: this.subPositionId }
    const dialogRef = this._dialog.open(CloneCapabilitySettingComponent, {
      header: `Clone capabilities`,
      width: "55%",
      contentStyle: { "max-height": "100%", overflow: 'visible' },
      baseZIndex: 5000,
      data: dialogData,
    });

    dialogRef.onClose.subscribe((res: CapabilitySettingClone) => {
      res && (this.userType = res.toUserType);
      res && (this.subPositionId = res.toSubPositionId);
      this.getListCapabilities();
    });
  }

  goBack() {
    this.router.navigate(["/app/categories/capability-setting"]);
  }

  addCapability(obj: CapabilitySettingCreateResponse) {
    const payload: CapabilitySettingPayload = {
      id: 0,
      userType: this.userType,
      subPositionId: this.subPositionId,
      capabilityId: obj.capabilityId,
      factor: obj.factor,
      guideLine: obj.guideLine,
      isDeleted: obj.isDeleted
    };

    this.subs.add(
      this._capabilitySetting.create(payload).subscribe(rs => {
        if (!rs.loading && rs.success) {
          const index = this.selectedList.findIndex(item => item.capabilityName === rs.result?.capabilityName)
          this.selectedList[index] = {
            ...this.selectedList[index],
            id: rs.result.id,
            subPositionId: rs.result.subPositionId,
            subPositionName: rs.result.subPositionName,
            userType: rs.result.userType,
            userTypeName: rs.result.userTypeName,
            factor: rs.result.factor,
            isDeleted: rs.result.isDeleted
          }
          this.getListCapabilities();
          this.showToastMessage(ToastMessageType.SUCCESS, 'Add capability success', obj.capabilityName);
        }
      })
    )
  }

  private handleSelectChange(userType: number, subPositionId: number) {
    this.isLoading = true;
    this.userType = checkNumber(userType) ? userType : null;
    this.subPositionId = checkNumber(subPositionId) ? subPositionId : null;

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { subPositionId: this.subPositionId, userType: this.userType },
      queryParamsHandling: 'merge',
    });
    this.getListCapabilities();
  }

  private getListCapabilities() {
    if (checkNumber(this.userType) && checkNumber(this.subPositionId)) {
      const payload = { userType: this.userType, subPositionId: this.subPositionId }
      forkJoin([
        this._capabilitySetting.getCapabilitiesByUserTypeAndPositionId(payload),
        this._capabilitySetting.GetRemainCapabilitiesByUserTypeAndPositionId(payload)
      ]).subscribe({
        next: ([selected, remainList]) => {
          this.isLoading = (selected.loading && remainList.loading);
          this.selectedList = selected.result;
          this.availableList = remainList.result;
        }
      })
    }
  }

  updateFactor(item: CapabilitySettingCreateResponse) {
    this._capabilitySetting.update(item).subscribe(rs => {
      if (!rs.loading && rs.success) {
        this.showToastMessage(ToastMessageType.SUCCESS, 'Update capability success', item.capabilityName);
      }
    })
  }
}
