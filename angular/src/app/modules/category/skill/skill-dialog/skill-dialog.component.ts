import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Skill, SkillConfigDiaLog } from '@app/core/models/categories/skill.model';
import { MESSAGE } from '@shared/AppConsts';
import { ToastMessageType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ActionEnum } from '../../../../../shared/AppEnums';
import { SkillService } from './../../../../core/services/categories/skill.service';

@Component({
  selector: 'talent-skill-dialog',
  templateUrl: './skill-dialog.component.html',
  styleUrls: ['./skill-dialog.component.scss']
})
export class SkillDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  skillDialogConfig: SkillConfigDiaLog;
  action: ActionEnum;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private injector: Injector,
    private _fb: FormBuilder,
    private _skill: SkillService
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.skillDialogConfig = this.config.data;
    this.action = this.skillDialogConfig.action;
    this.initForm();
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const action = this.skillDialogConfig.action;
    const payload: Skill = {
      id: action === ActionEnum.UPDATE ? this.skillDialogConfig.skill.id : 0,
      name: this.formControls['name'].value
    };
    this.handleSave(payload, action,isClose);

  }

  checkAndClosePopup(res: ApiResponse<Skill>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    this.form = this._fb.group({
      name: [
        this.skillDialogConfig.action == ActionEnum.UPDATE ? this.skillDialogConfig.skill.name : '',
        [Validators.required]
      ]
    });
  }

  private resetForm() {
    this.initForm();
    this.submitted = false;
  }
  
  private handleSave(payload: Skill, action: ActionEnum,isClose: boolean) {
    if (action === ActionEnum.CREATE) {
      this.subs.add(
        this._skill.create(payload).subscribe(res => {
          this.isLoading = res.loading;
          if (res.loading) return;
          this.doSave(res,isClose);
        })
      );
      return;
    }

    this.subs.add(
      this._skill.update(payload).subscribe(res => {
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
