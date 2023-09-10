import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue } from '@app/core/helpers/utils.helper';
import { PositionSettingService } from '@app/core/services/categories/position-setting.service';
import { MESSAGE } from '@shared/AppConsts';
import { DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ApiResponse } from '../../../../../shared/paged-listing-component-base';
import { ActionEnum, ToastMessageType } from './../../../../../shared/AppEnums';
import { PositionSetting, PositionSettingCreate } from './../../../../core/models/categories/position-setting.model';
import { UtilitiesService } from './../../../../core/services/utilities.service';

@Component({
  selector: 'talent-position-setting-dialog',
  templateUrl: './position-setting-dialog.component.html',
  styleUrls: ['./position-setting-dialog.component.scss']
})
export class PositionSettingDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  action: ActionEnum;

  constructor(
    injector: Injector,
    public ref: DynamicDialogRef,
    public _utilities: UtilitiesService,
    private _fb: FormBuilder,
    private _positionSetting: PositionSettingService,
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.action = ActionEnum.CREATE;
    this.initForm();
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const payload: PositionSettingCreate = {
      userType: getFormControlValue(this.form, 'userType'),
      subPositionId: getFormControlValue(this.form, 'subPositionId'),
    };
    this.handleSave(payload, isClose);
  }

  onPositionSelect(subPositionId: number) {
    this.formControls.subPositionId.setValue(subPositionId);
  }


  checkAndClosePopup(res: ApiResponse<PositionSetting>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    this.form = this._fb.group({
      userType: [null, [Validators.required]],
      subPositionId: [null, [Validators.required]],
    });
  }

  private resetForm() {
    this.initForm();
    this.submitted = false;
  }

  private handleSave(payload: PositionSettingCreate, isClose: boolean) {
    this.subs.add(
      this._positionSetting.create(payload).subscribe(res => {
        this.isLoading = res.loading;
        if (res.loading) return;
        this.doSave(res, isClose);
      })
    );
  }
  private doSave(res, isClose: boolean) {
    this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS,
      `${res.result.userTypeName} - ${res.result.subPositionName}`);
    if (res.success && isClose) {
      this.checkAndClosePopup(res);
      return;
    }
    this.resetForm();
  }
}
