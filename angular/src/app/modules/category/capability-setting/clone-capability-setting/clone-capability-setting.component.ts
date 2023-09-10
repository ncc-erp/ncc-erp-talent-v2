import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue } from '@app/core/helpers/utils.helper';
import { CapabilitySettingClone, CloneCapabilitySettingConfigDiaLog } from '@app/core/models/categories/capabilities-setting.model';
import { CapabilitySettingService } from '@app/core/services/categories/capability-setting.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { MESSAGE } from '@shared/AppConsts';
import { ToastMessageType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-clone-capability-setting',
  templateUrl: './clone-capability-setting.component.html',
  styleUrls: ['./clone-capability-setting.component.scss']
})
export class CloneCapabilitySettingComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  dialogData: CloneCapabilitySettingConfigDiaLog;


  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    public _utilities: UtilitiesService,
    private injector: Injector,
    private _fb: FormBuilder,
    private _capablitySetting: CapabilitySettingService
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.dialogData = this.config.data;
    this.initForm();
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const payload: CapabilitySettingClone = {
      fromUserType: getFormControlValue(this.form, 'userTypeFrom'),
      fromSubPositionId: getFormControlValue(this.form, 'subPositionIdFrom'),
      toUserType: getFormControlValue(this.form, 'userTypeTo'),
      toSubPositionId: getFormControlValue(this.form, 'subPositionIdTo'),
    };
    this.handleSave(payload, isClose);
  }

  onPositionSelect(subPositionId: number, isForm: boolean) {
    isForm ? this.formControls.subPositionIdFrom.setValue(subPositionId) : this.formControls.subPositionIdTo.setValue(subPositionId)
  }

  checkAndClosePopup(res: ApiResponse<any>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    const userTypeFrom = this.dialogData.userTypeFrom ?? null;
    const subPositionIdFrom = this.dialogData.subPositionIdFrom ?? null;
    this.form = this._fb.group({
      userTypeFrom: [userTypeFrom, [Validators.required]],
      subPositionIdFrom: [subPositionIdFrom, [Validators.required]],
      userTypeTo: [null, [Validators.required]],
      subPositionIdTo: [null, [Validators.required]],
    });
  }

  private resetForm() {
    this.form.reset({ ...this.form.value, subPositionIdTo: null })
    this.submitted = false;
  }

  private handleSave(payload: CapabilitySettingClone, isClose: boolean) {
    this.subs.add(
      this._capablitySetting.capabilitySettingClone(payload).subscribe(res => {
        this.isLoading = res.loading;
        if (res.loading || !res.success) return;

        const result = res.result;
        const msg = `From: ${result.fromUserTypeName} ${result.fromSubPositionName} To: ${result.toUserTypeName} ${result.toSubPositionName}`;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CLONE_SUCCESS, msg);
        if (isClose) {
          this.ref.close(result);
          return;
        }
        this.resetForm();
      })
    );
  }
}
