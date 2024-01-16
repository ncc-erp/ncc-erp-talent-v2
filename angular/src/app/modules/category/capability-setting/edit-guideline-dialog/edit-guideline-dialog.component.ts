import { ToastMessageType } from '../../../../../shared/AppEnums';
import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiResponse } from '../../../../../shared/paged-listing-component-base';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ActionEnum } from '../../../../../shared/AppEnums';
import { MESSAGE } from '../../../../../shared/AppConsts';
import { CapabilitySetting, CapabilitySettingConfigDiaLog, CapabilitySettingPayload } from '@app/core/models/categories/capabilities-setting.model';
import { CapabilitySettingService } from '@app/core/services/categories/capability-setting.service';
import { DataSharingService } from '@app/core/services/categories/data-sharing-capabillity.service';

@Component({
  selector: 'talent-edit-guideline-dialog',
  templateUrl: './edit-guideline-dialog.component.html',
  styleUrls: ['./edit-guideline-dialog.component.scss']
})
export class EditGuideLineDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  capabilitySettingConfigDialog: CapabilitySettingConfigDiaLog;
  action: ActionEnum;
  isEditable: boolean;
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private injector: Injector,
    private _fb: FormBuilder,
    private _capabilitySetting: CapabilitySettingService,
    private dataSharingService: DataSharingService
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.capabilitySettingConfigDialog = this.config.data;
    this.action = this.capabilitySettingConfigDialog.action;
    this.isEditable = true;
    this.initForm();
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;

    const action = this.capabilitySettingConfigDialog.action;
    const payload: CapabilitySettingPayload = {
      id: action === ActionEnum.UPDATE ? this.capabilitySettingConfigDialog.capabilityWithSetting.id: 0,
      guideLine: this.formControls['guideline'].value,
      capabilityId: this.capabilitySettingConfigDialog.capabilityWithSetting.capabilityId,
      userType: this.capabilitySettingConfigDialog.capabilitySetting.userType,
      subPositionId: this.capabilitySettingConfigDialog.capabilitySetting.subPositionId,
      factor: this.capabilitySettingConfigDialog.capabilityWithSetting.factor,
      isDeleted: false,
    };
    this.handleSave(payload, action, isClose);
  }

  checkAndClosePopup(res: ApiResponse<CapabilitySetting>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    this.form = this._fb.group({
      name: [
        this.capabilitySettingConfigDialog.action == ActionEnum.UPDATE ? this.capabilitySettingConfigDialog.capabilityWithSetting.capabilityName : '',
        [Validators.required]
      ],
      guideline: [
        this.capabilitySettingConfigDialog.action == ActionEnum.UPDATE ? this.capabilitySettingConfigDialog.capabilityWithSetting.guideLine: '',
        [Validators.maxLength(5000)]
      ],
    });
  }

  private resetForm() {
    this.initForm();
    this.submitted = false;
  }

  private handleSave(payload: CapabilitySettingPayload, action: ActionEnum, isClose: boolean) {
    if (action === ActionEnum.CREATE) {
      this.subs.add(
        this._capabilitySetting.create(payload).subscribe(res =>  {
          this.isLoading = res.loading;
          if (res.loading) return;
          this.doSave(res,isClose);
        })
      );
      return;
    }

    this.subs.add(
      this._capabilitySetting.update(payload).subscribe(res => {
        this.isLoading = res.loading;      
         this.dataSharingService.updatePayload(payload);
        this.checkAndClosePopup(res)
      })
    );
  }

  private doSave(res,isClose:boolean) {
    this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS, res.result.name);
    if(res.success && isClose){
      this.checkAndClosePopup(res);
      return;
    }
    this.resetForm();
  }
}
