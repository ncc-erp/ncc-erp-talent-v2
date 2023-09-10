import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue } from '@app/core/helpers/utils.helper';
import { ProfileEducation } from '@app/core/models/employee-profile/profile-model';
import { MyProfileService } from '@app/core/services/employee-profile/my-profile.service';
import { AppComponentBase } from '@shared/app-component-base';
import { EMPLOYEE_PROFILE, MESSAGE } from '@shared/AppConsts';
import { ToastMessageType, ActionEnum } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { AppSessionService } from '@shared/session/app-session.service';
import { PermissionCheckerService } from 'abp-ng2-module';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { findDegreeId } from '../../employee-profile.helper';

@Component({
  selector: 'talent-profile-education',
  templateUrl: './profile-education.component.html',
  styleUrls: ['./profile-education.component.scss']
})
export class ProfileEducationComponent extends AppComponentBase implements OnInit {

  education: ProfileEducation;
  listDegree = EMPLOYEE_PROFILE.Degree;
  isSale: boolean;
  isUser: boolean;
  isEmployee = false;
  id: number;

  form: FormGroup;
  submitted = false;
  action: ActionEnum;
  isUpdate: boolean = false;

  constructor(
    injector: Injector,
    private _myProfile: MyProfileService,
    public config: DynamicDialogConfig,
    public dialogRef: DynamicDialogRef,
    private _permissionChecker: PermissionCheckerService,
    private session: AppSessionService,
    private _fb: FormBuilder,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.isEmployee === false) this.id = this.session.userId;
    this.isSale = this._permissionChecker.isGranted('Pages.EditAsSales.Employee');

    Object.assign(this, this.config.data)
    this.isUpdate = this.action === ActionEnum.UPDATE;
    this.initForm();
  }

  get formControls() { return this.form.controls; }

  submitEducation() {
    this.submitted = true;
    if (this.form.invalid) return;

    this.education = { ...this.education, ...this.form.getRawValue() };
    if (this.isSale && !this.isUser) {
      this.dialogRef.close(this.education); return;
    }

    if (getFormControlValue(this.form, 'endYear') - getFormControlValue(this.form, 'startYear') < 0) {
      this.showToastMessage(ToastMessageType.ERROR, 'Wrong input', 'Year end must be greater than start')
      return;
    }

    this._myProfile.saveEducation(this.education).subscribe(res => {
      this.isLoading = res.loading;
      res.success && this.dialogRef.close(res);
    })
  };

  private initForm() {
    const startYear = this.education?.startYear ? this.education.startYear : null;
    const endYear = this.education?.endYear ? this.education.endYear : null;
    const schoolOrCenterName = this.education?.schoolOrCenterName ? this.education.schoolOrCenterName : '';
    const degreeType = (this.isUpdate && this.education?.degreeType) ? findDegreeId(this.education.degreeType) : null;
    const major = this.education?.major ? this.education.major : '';

    this.form = this._fb.group({
      startYear: [startYear, [Validators.required, Validators.min(4)]],
      endYear: [endYear, [Validators.required, Validators.min(4)]],
      schoolOrCenterName: [schoolOrCenterName, [Validators.required]],
      degreeType: [degreeType, [Validators.required]],
      major: [major, [Validators.required]],
    });
  }

}
