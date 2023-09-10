import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue } from '@app/core/helpers/utils.helper';
import { LmsCourse, PositionSettingUpdate } from '@app/core/models/categories/position-setting.model';
import { PositionSettingService } from '@app/core/services/categories/position-setting.service';
import { MESSAGE } from '@shared/AppConsts';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ApiResponse } from '../../../../../shared/paged-listing-component-base';
import { ToastMessageType } from './../../../../../shared/AppEnums';
import { PositionSetting } from './../../../../core/models/categories/position-setting.model';
import { UtilitiesService } from './../../../../core/services/utilities.service';

@Component({
  selector: 'talent-lms-setting-dialog',
  templateUrl: './lms-setting-dialog.component.html',
  styleUrls: ['./lms-setting-dialog.component.scss']
})
export class LmsSettingDialogComponent extends AppComponentBase implements OnInit {

  form: FormGroup;
  submitted: boolean = false;
  isCreating: boolean = false;
  positionSettingUpdate: PositionSettingUpdate;

  constructor(
    injector: Injector,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
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
    this.positionSettingUpdate = this.config.data;
    this.initForm();
  }

  getCourse(): LmsCourse {
    return getFormControlValue(this.form, 'lmsCourse')
  }

  onSave() {
    this.submitted = true;
    if (this.form.invalid) return;
    const payload: PositionSettingUpdate = {
      ...this.positionSettingUpdate,
      lmsCourseId: this.getCourse()?.id,
      lmsCourseName: this.getCourse()?.name,
    };
    this.handleSave(payload);
  }

  checkAndClosePopup(res: ApiResponse<PositionSetting>) {
    this.isCreating = res.loading;
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    const lmsCourse = this._utilities.catLmsCourse.find(item => item.id === this.positionSettingUpdate.lmsCourseId);

    this.form = this._fb.group({
      lmsCourse: [lmsCourse || '', [Validators.required]],
    });
  }

  private handleSave(payload: PositionSettingUpdate) {
    this.subs.add(
      this._positionSetting.update(payload).subscribe(res => {
        res.success && this.checkAndClosePopup(res);
      })
    );
  }
}
