import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue, randomHexColor } from '@app/core/helpers/utils.helper';
import { JobPosition, JobPositionConfigDiaLog } from '@app/core/models/categories/job-position.model';
import { JobPositionService } from '@app/core/services/categories/job-position.service';
import { MESSAGE } from '@shared/AppConsts';
import { ToastMessageType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ActionEnum } from '../../../../../shared/AppEnums';

@Component({
  selector: 'talent-job-position-dialog',
  templateUrl: './job-position-dialog.component.html',
  styleUrls: ['./job-position-dialog.component.scss']
})
export class JobPositionDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  jobPositionDialogConfig: JobPositionConfigDiaLog;
  action: ActionEnum;
  showButtonSave: boolean = true;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private injector: Injector,
    private _fb: FormBuilder,
    private _jobPosition: JobPositionService
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.jobPositionDialogConfig = this.config.data;
    this.action = this.jobPositionDialogConfig.action;
    this.showButtonSave = this.jobPositionDialogConfig.showButtonSave;
    this.initForm();
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const action = this.jobPositionDialogConfig.action;
    const payload: JobPosition = {
      id: action === ActionEnum.UPDATE ? this.jobPositionDialogConfig.jobPosition.id : 0,
      colorCode: getFormControlValue(this.form, 'colorCode'),
      name: getFormControlValue(this.form, 'name')
    };
    this.handleSave(payload, action, isClose);

  }

  checkAndClosePopup(res: ApiResponse<JobPosition>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    const isUpdate = this.jobPositionDialogConfig.action == ActionEnum.UPDATE;
    const hexColor = isUpdate ? this.jobPositionDialogConfig.jobPosition.colorCode : randomHexColor();

    this.form = this._fb.group({
      name: [
        isUpdate ? this.jobPositionDialogConfig.jobPosition.name : '',
        [Validators.required]
      ],
      colorCode: [hexColor, [Validators.required]],
      colorCodeIp: [hexColor, [Validators.required]]
    });

    this.formControls.colorCodeIp.valueChanges.subscribe(color => {
      if (!color.startsWith('#')) color = `#${color}`
      this.formControls.colorCode.patchValue(color, { emitEvent: false });
    })

    this.formControls.colorCode.valueChanges.subscribe(color => {
      this.formControls.colorCodeIp.patchValue(color, { emitEvent: false });
    })
  }

  private resetForm() {
    this.initForm();
    this.submitted = false;
  }

  private handleSave(payload: JobPosition, action: ActionEnum, isClose: boolean) {
    if (action === ActionEnum.CREATE) {
      this.subs.add(
        this._jobPosition.create(payload).subscribe(res => {
          this.isLoading = res.loading;
          if (res.loading) return;
          this.doSave(res,isClose);
        })
      );
      return;
    }

    this.subs.add(
      this._jobPosition.update(payload).subscribe(res => {
        this.isLoading = res.loading;
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
