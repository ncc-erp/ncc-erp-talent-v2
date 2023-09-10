import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue, randomHexColor } from '@app/core/helpers/utils.helper';
import { MESSAGE } from '@shared/AppConsts';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ActionEnum } from '../../../../../shared/AppEnums';
import { ApiResponse } from '../../../../../shared/paged-listing-component-base';
import { ToastMessageType } from './../../../../../shared/AppEnums';
import { UtilitiesService } from './../../../../core/services/utilities.service';

import { FakeSkill,FakeSkillConfigDiaLog } from './../../../../core/models/employee-profile/fake-skill-model';
import { FakeSkillService } from './../../../../core/services/employee-profile/fake-skill.service';
import { GroupSkillDialogComponent } from '../../group-skill/group-skill-dialog/group-skill-dialog.component';
import { GroupSkillService } from '@app/core/services/employee-profile/group-skill.service'; 
import { GroupSkill,GroupSkillConfigDiaLog } from '@app/core/models/employee-profile/group-skill-model'; 

@Component({
  selector: 'talent-fake-skill-dialog',
  templateUrl: './fake-skill-dialog.component.html',
  styleUrls: ['./fake-skill-dialog.component.scss']
})
export class FakeSkillDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  fakeSkillConfigDiaLog: FakeSkillConfigDiaLog;
  action: ActionEnum;
  public groupSkills: GroupSkill[] = [];
  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    public _utilities: UtilitiesService,
    public dialogService: DialogService,
    private injector: Injector,
    private _fb: FormBuilder,
    private _fakeSkillService: FakeSkillService,
    private _groupSkillService: GroupSkillService,
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.fakeSkillConfigDiaLog = this.config.data;
    this.action = this.fakeSkillConfigDiaLog.action;
    this.initForm();
    this.getAllGroupSkill();
  }

  getAllGroupSkill() {
    this.subs.add(
      this._groupSkillService.getAll().subscribe((rs) => rs.success && (this.groupSkills = rs.result))
    );
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const action = this.fakeSkillConfigDiaLog.action;
    const payload: FakeSkill = {
      id: action === ActionEnum.UPDATE ? this.fakeSkillConfigDiaLog.fakeSkill.id : 0,
      name: getFormControlValue(this.form, 'name'),
      description: getFormControlValue(this.form, 'description'),
      groupSkillId: getFormControlValue(this.form, 'groupSkillId'),
    };
    this.handleSave(payload, action,isClose);
  }

  openMainGroupSkillDialog() {
    const dialogConfig: GroupSkillConfigDiaLog = { groupSkill: null, action: ActionEnum.CREATE,showButtonSave:false};
    const dialogRef = this.dialogService.open(GroupSkillDialogComponent, {
      header: `${dialogConfig.action} Group Skill`,
      width: "40%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<GroupSkill>) => {
      if (res) {
        const newGroupSkill: GroupSkill = res.result;
        this.formControls['groupSkillId'].setValue(newGroupSkill.id);
        this.getAllGroupSkill()
      }
    });
  }

  checkAndClosePopup(res: ApiResponse<FakeSkill>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    const isUpdate = this.fakeSkillConfigDiaLog.action === ActionEnum.UPDATE;

    this.form = this._fb.group({
      name: [
        isUpdate ? this.fakeSkillConfigDiaLog.fakeSkill.name : '',
        [Validators.required]
      ],
      description: [
        isUpdate ? this.fakeSkillConfigDiaLog.fakeSkill.description : '',
        []
      ],
      groupSkillId: [
        isUpdate ? this.fakeSkillConfigDiaLog.fakeSkill.groupSkillId : '',
        [Validators.required]
      ],
     
    });
  }

  private resetForm() {
    this.form.reset({ ...this.form.value, name: '',description: '' })
    this.submitted = false;
  }

  private handleSave(payload: FakeSkill, action: ActionEnum,isClose: boolean) {
    if (action === ActionEnum.CREATE) {
      this.subs.add(
        this._fakeSkillService.create(payload).subscribe(res => {
          this.isLoading = res.loading;
          if (res.loading) return;
          this.doSave(res,isClose);
        })
      );
      return;
    }

    this.subs.add(
      this._fakeSkillService.update(payload).subscribe(res => {
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
