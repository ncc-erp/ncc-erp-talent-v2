import { Component, Injector, OnInit } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { SkillCandidateDto } from '@app/core/models/employee-profile/profile-model';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { EMPLOYEE_PROFILE } from '@shared/AppConsts';
import { ActionEnum, ToastMessageType } from '@shared/AppEnums';
import { PermissionCheckerService } from 'abp-ng2-module';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MyProfileService } from '../../../../core/services/employee-profile/my-profile.service';
import { AppSessionService } from './../../../../../shared/session/app-session.service';
import { GroupTechnical, ProfileCvSkill, TechnicalExpertise } from './../../../../core/models/employee-profile/profile-model';
import { CommonEmployeeService } from './../../../../core/services/employee-profile/common-employee.service';

@Component({
  selector: 'talent-technical-expertises',
  templateUrl: './technical-expertises.component.html',
  styleUrls: ['./technical-expertises.component.scss']
})
export class TechnicalExpertisesComponent extends AppComponentBase implements OnInit {
  technicalForm: FormGroup;
  technicalEditForm: FormGroup;
  listSkill: SkillCandidateDto[] = [];
  listData: TechnicalExpertise;
  listLevel = EMPLOYEE_PROFILE.DanhSach;
  listSkillChild: any[] = [];
  title: string;
  idGroupSkill: any;
  isSale: boolean;
  isUser: boolean;
  isEmployee = false;
  id: number;

  action: ActionEnum;
  form: FormGroup;

  submitted: boolean = false;

  constructor(
    injector: Injector,
    public config: DynamicDialogConfig,
    public dialogRef: DynamicDialogRef,
    public _utilities: UtilitiesService,
    private fb: FormBuilder,
    private _commonEmployee: CommonEmployeeService,
    private _myProfile: MyProfileService,
    private _permissionChecker: PermissionCheckerService,
    private session: AppSessionService,
  ) {
    super(injector);
  }


  ngOnInit(): void {
    this.initForm();
    if (this.isEmployee === false) {
      this.id = this.session.userId;
    }

    this.isSale = this._permissionChecker.isGranted('Pages.EditAsSales.Employee');
    this.listData = this.config.data?.listskill;
    this.isUser = this.config.data?.isUser;
    this.action = this.config.data?.action;
    if (this.action === ActionEnum.UPDATE && this.listData.groupSkills) {
      this.updateValueOnEdit()
    };

    if (!this.listData.groupSkills.length) this.addgroupSkills();
  }

  groupSkills(): FormArray {
    return this.form.get('groupSkills') as FormArray;
  }

  getSkillIndexByGroupSkill(groupSkillIndex: number): FormArray {
    return this.groupSkills().at(groupSkillIndex).get('cvSkills') as FormArray;
  }

  skills(index: number): FormArray {
    return this.groupSkills()?.at(index)?.get("cvSkills") as FormArray
  }

  getOptionSkills(formGroupSkillIndex: number) {
    return this.groupSkills().at(formGroupSkillIndex).get('optionSkills').value;
  }

  getSkillByGroupSkillId(id: number, currentFormGroupSkill: AbstractControl) {
    this.listSkill = [];
    this._commonEmployee.getSkillByGroupSkillId(id).subscribe(res => {
      if (!res || res.loading || !res.result) return;
      currentFormGroupSkill.patchValue({ optionSkills: res.result })
    });
  }

  changeGroupSkillId(groupSkillName: string, groupSkillIndex: number) { //OK 
    const id = this._utilities.catCCBGroupSkill.find(item => item.name === groupSkillName)?.id;
    const currentFormGroupSkill = this.groupSkills().at(groupSkillIndex);
    currentFormGroupSkill.patchValue({
      groupSkillId: this._utilities.catCCBGroupSkill.find(item => item.name === groupSkillName)?.id
    })
    this.getSkillByGroupSkillId(id, currentFormGroupSkill);
  }

  changeListSkillId(skillName: string, skillIndex: number, groupSkillIndex: number) {
    const currentFormGroupSkill = this.groupSkills().at(groupSkillIndex);
    const optionSkills: any[] = currentFormGroupSkill.get('optionSkills').value;
    const id = optionSkills.find(item => item.name === skillName).id
    const currentSkill = this.getSkillIndexByGroupSkill(groupSkillIndex).at(skillIndex);
    currentSkill.patchValue({
      skillId: id
    })
  }

  deleteCvSkill(groupSkillIndex: number, skillIndex: number) {
    this.getSkillIndexByGroupSkill(groupSkillIndex).removeAt(skillIndex);
  }

  addCvSkill(groupSkillIndex: number) {
    this.skills(groupSkillIndex).push(this.addFormSkill());
  }

  saveExpertise() {
    this.submitted = true;
    if (this.form.invalid){
      return;
    }
    if (this.isSale && !this.isUser) {
      this.dialogRef.close(this.listData);
      return;
    }

    const payloadItem = this.groupSkills().getRawValue();
    payloadItem.forEach(item => delete item.optionSkills);

    const payload: TechnicalExpertise = {
      userId: this.listData.userId,
      groupSkills: payloadItem

    }
    this._myProfile.updateTechnicalExpertise(payload).subscribe(res => {
      this.isLoading = res.loading;
      res.success && this.dialogRef.close(res);
    })
  }

  addgroupSkills(data: GroupTechnical = null) {
    const formGroupSkill = this.fb.group({
      groupSkillId: data?.groupSkillId ?? null,
      name: data?.name ?? '',
      cvSkills: this.fb.array([]),
      optionSkills: [],
    });
    if (!data) {
      const formSkills = formGroupSkill.get('cvSkills') as FormArray;
      formSkills.push(this.addFormSkill());
      this.groupSkills().push(formGroupSkill);
      return;
    }
    
    //edit
    const skills: ProfileCvSkill[] = data.cvSkills;
    const formSkills = formGroupSkill.get('cvSkills') as FormArray;
    skills.forEach(skill => formSkills.push(this.addFormSkill(skill)))

    this.groupSkills().push(formGroupSkill);
    this.getSkillByGroupSkillId(data.groupSkillId, formGroupSkill);
  }

  addFormSkill(data: ProfileCvSkill = null) {
    return this.fb.group({
      id: data?.id ?? null,
      level: data?.level ?? 1,
      order: data?.order ?? null,
      skillId: data?.skillId ?? null,
      skillName: data?.skillName ?? ''
    })
  }

  private initForm() {
    this.form = this.fb.group({
      groupSkills: this.fb.array([])
    });
  }

  private updateValueOnEdit() {
    const groupSkills: GroupTechnical[] = this.listData.groupSkills;
    groupSkills.forEach(groupSkill => this.addgroupSkills(groupSkill))

  }
}
