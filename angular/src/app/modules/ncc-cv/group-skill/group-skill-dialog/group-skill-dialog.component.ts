import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue } from '@app/core/helpers/utils.helper';
import { MESSAGE } from '@shared/AppConsts';
import { ToastMessageType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ActionEnum } from '../../../../../shared/AppEnums';
import { GroupSkill,GroupSkillConfigDiaLog } from './../../../../core/models/employee-profile/group-skill-model';
import { GroupSkillService } from './../../../../core/services/employee-profile/group-skill.service';



@Component({
  selector: 'talent-group-skill-dialog',
  templateUrl: './group-skill-dialog.component.html',
  styleUrls: ['./group-skill-dialog.component.scss']
})
export class GroupSkillDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  groupSkillConfigDiaLog: GroupSkillConfigDiaLog;
  action: ActionEnum;
  showButtonSave = true;
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private injector: Injector,
    private _fb: FormBuilder,
    private _employeePositionService: GroupSkillService
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.groupSkillConfigDiaLog = this.config.data;
    this.action = this.groupSkillConfigDiaLog.action;
    this.showButtonSave = this.groupSkillConfigDiaLog.showButtonSave;
    this.initForm();
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const action = this.groupSkillConfigDiaLog.action;
    const payload: GroupSkill = {
      id: action === ActionEnum.UPDATE ? this.groupSkillConfigDiaLog.groupSkill.id : 0,
      name: getFormControlValue(this.form, 'name'),
      default: getFormControlValue(this.form, 'default'),
    };
    this.handleSave(payload, action,isClose);

  }

  checkAndClosePopup(res: ApiResponse<GroupSkill>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    this.form = this._fb.group({
      name: [
        this.groupSkillConfigDiaLog.action == ActionEnum.UPDATE ? this.groupSkillConfigDiaLog.groupSkill.name : '',
        [Validators.required]
      ],
      default: [
        this.groupSkillConfigDiaLog.action == ActionEnum.UPDATE ? this.groupSkillConfigDiaLog.groupSkill.default : false,
        []
      ]
    });
  }

  private resetForm() {
    this.initForm();
    this.submitted = false;
  }
  
  private handleSave(payload: GroupSkill, action: ActionEnum,isClose: boolean) {
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
