import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EducationType, EducationTypeConfigDiaLog } from '@app/core/models/categories/education-type.model';
import { EducationTypeService } from '@app/core/services/categories/education-type.service';
import { MESSAGE } from '@shared/AppConsts';
import { ToastMessageType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ActionEnum } from '../../../../../shared/AppEnums';

@Component({
  selector: 'talent-education-types-dialog',
  templateUrl: './education-types-dialog.component.html',
  styleUrls: ['./education-types-dialog.component.scss']
})
export class EducationTypesDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  eduTypeDialogConfig: EducationTypeConfigDiaLog;
  action: ActionEnum;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private injector: Injector,
    private _fb: FormBuilder,
    private _educationType: EducationTypeService
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.eduTypeDialogConfig = this.config.data;
    this.action = this.eduTypeDialogConfig.action;
    this.initForm();
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const action = this.eduTypeDialogConfig.action;
    const payload: EducationType = {
      id: action === ActionEnum.UPDATE ? this.eduTypeDialogConfig.educationType.id : 0,
      name: this.formControls['name'].value
    };
    this.handleSave(payload, action, isClose);
  }

  checkAndClosePopup(res: ApiResponse<EducationType>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    this.form = this._fb.group({
      name: [
        this.eduTypeDialogConfig.action == ActionEnum.UPDATE ? this.eduTypeDialogConfig.educationType.name : '',
        [Validators.required]
      ]
    });
  }

  private resetForm() {
    this.initForm();
    this.submitted = false;
  }
  
  private handleSave(payload: EducationType, action: ActionEnum, isClose: boolean) {
    if (action === ActionEnum.CREATE) {
      this.subs.add(
        this._educationType.create(payload).subscribe(res =>  {
          this.isLoading = res.loading;
          if (res.loading) return;
          this.doSave(res,isClose);
        })
      );
      return;
    }

    this.subs.add(
      this._educationType.update(payload).subscribe(res => {
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
