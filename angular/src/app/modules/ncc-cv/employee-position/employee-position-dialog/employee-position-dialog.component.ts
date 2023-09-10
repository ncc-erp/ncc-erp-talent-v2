import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue } from '@app/core/helpers/utils.helper';
import { MESSAGE } from '@shared/AppConsts';
import { ToastMessageType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ActionEnum } from '../../../../../shared/AppEnums';
import { EmployeePosition,EmployeePositionConfigDiaLog } from './../../../../core/models/employee-profile/employee-position-model';
import { EmployeePositionService } from './../../../../core/services/employee-profile/employee-position.service';


@Component({
  selector: 'talent-employee-position-dialog',
  templateUrl: './employee-position-dialog.component.html',
  styleUrls: ['./employee-position-dialog.component.scss']
})
export class EmployeePositionDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  employeePositionConfigDiaLog: EmployeePositionConfigDiaLog;
  action: ActionEnum;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private injector: Injector,
    private _fb: FormBuilder,
    private _employeePositionService: EmployeePositionService
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.employeePositionConfigDiaLog = this.config.data;
    this.action = this.employeePositionConfigDiaLog.action;
    this.initForm();
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const action = this.employeePositionConfigDiaLog.action;
    const payload: EmployeePosition = {
      id: action === ActionEnum.UPDATE ? this.employeePositionConfigDiaLog.employeePosition.id : 0,
      name: getFormControlValue(this.form, 'name'),
      description: getFormControlValue(this.form, 'description'),
    };
    this.handleSave(payload, action,isClose);

  }

  checkAndClosePopup(res: ApiResponse<EmployeePosition>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    this.form = this._fb.group({
      name: [
        this.employeePositionConfigDiaLog.action == ActionEnum.UPDATE ? this.employeePositionConfigDiaLog.employeePosition.name : '',
        [Validators.required]
      ],
      description: [
        this.employeePositionConfigDiaLog.action == ActionEnum.UPDATE ? this.employeePositionConfigDiaLog.employeePosition.description : '',
        []
      ]
    });
  }

  private resetForm() {
    this.initForm();
    this.submitted = false;
  }
  
  private handleSave(payload: EmployeePosition, action: ActionEnum,isClose: boolean) {
    if (action === ActionEnum.CREATE) {
      this.subs.add(
        this._employeePositionService.create(payload).subscribe(res => {
          this.isLoading = res.loading;
          if (res.loading) return;
          this.doSave(res,isClose);
        })
      );
      return;
    }

    this.subs.add(
      this._employeePositionService.update(payload).subscribe(res => {
        this.isLoading = res.loading;
        this.checkAndClosePopup(res)
      })
    );
  }
  private doSave(res,isClose:boolean) {
    this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS, res.result.displayName);
    if(res.success && isClose){
      this.checkAndClosePopup(res);
      return;
    }
    this.resetForm();
  }
}
