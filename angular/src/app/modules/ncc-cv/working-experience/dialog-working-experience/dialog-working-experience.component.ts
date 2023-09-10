import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue } from '@app/core/helpers/utils.helper';
import { VesionDto, WorkingExperience } from '@app/core/models/employee-profile/profile-model';
import { EmployeeProfileService } from '@app/core/services/employee-profile/employee-profile.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { DateFormat } from '@shared/AppConsts';
import { ActionEnum, ToastMessageType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { AppSessionService } from '@shared/session/app-session.service';
import { PermissionCheckerService } from 'abp-ng2-module';
import * as moment from 'moment';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MyProfileService } from '@app/core/services/employee-profile/my-profile.service';
import { HttpErrorResponse } from '@angular/common/http';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Component({
  selector: 'talent-dialog-working-experience',
  templateUrl: './dialog-working-experience.component.html',
  styleUrls: ['./dialog-working-experience.component.scss']
})

export class DialogWorkingExperienceComponent extends AppComponentBase implements OnInit {

  workingExperience: WorkingExperience;
  listVersions: VesionDto[] = [];

  isEmployee = false;
  id: number;
  isSaving: boolean = false;
  form: FormGroup;
  submitted = false;
  action: ActionEnum;
  currentlyWorking = false;
  isResult = false;
  isNoResult = false;

  versionPositionId = null;
  versionLanguageId = null;


  constructor(
    injector: Injector,
    private _employeeProfileService: EmployeeProfileService,
    private _myProfileService: MyProfileService,
    public config: DynamicDialogConfig,
    public dialogRef: DynamicDialogRef,
    private _permissionChecker: PermissionCheckerService,
    private session: AppSessionService,
    private _fb: FormBuilder,
    public ref: DynamicDialogRef,
    public _utilities: UtilitiesService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getDialogConfig();
    this.initForm();
    this.setValueCurrentlyWorking();
  }



  private getDialogConfig() {
    this.action = this.config.data.action;
    this.workingExperience = this.config.data.workingExperience;
  }

  get formControls() { return this.form.controls; }

  searchVersion() {
    const data = {
      versionName: getFormControlValue(this.form, 'versionName'),
      versionPositionId: getFormControlValue(this.form, 'versionPositionId') == null ? "":getFormControlValue(this.form, 'versionPositionId') ,
      versionLanguageId: getFormControlValue(this.form, 'versionLanguageId') == null ? "":getFormControlValue(this.form, 'versionLanguageId') ,
    };
    
    this._employeeProfileService
      .getEmployeeVersFilter(data.versionName, data.versionPositionId, data.versionLanguageId).subscribe(res => {
        if (res.result) {
          this.listVersions = res.result;
          if (this.listVersions.length !== 0) {
            this.isResult = true;
            this.isNoResult = false;
          } else {
            this.isResult = false;
            this.isNoResult = true;
          }
        }
      });
  }

  selectedProjectName(item: VesionDto, event, index) {
    this.listVersions[index].isChecked = event.target.checked;
  }

  addVersionDefault() {
    this.submitted = true;
    if (this.form.invalid) return;
    const payload = {
      id: this.workingExperience.id,
      startTime: moment(this.formControls['startTime'].value).format(DateFormat.YYYY_MM_DD),
      endTime: this.currentlyWorking ? null : moment(this.formControls['endTime'].value).format(DateFormat.YYYY_MM_DD),
      projectId: this.workingExperience.projectId,
      projectName: getFormControlValue(this.form, 'projectName'),
      technologies: getFormControlValue(this.form, 'technologies'),
      position: getFormControlValue(this.form, 'position'),
      projectDescription: getFormControlValue(this.form, 'projectDescription'),
      responsibility: getFormControlValue(this.form, 'responsibility'),
      userId: this.workingExperience.userId,
      order: this.workingExperience.order,
      versionId: this.workingExperience.versionId
    } as WorkingExperience;

    this.handleSave(payload);
  }


  addVersion() {
    const listChecked = this.listVersions.filter(el => el.isChecked === true);
    let promises = [];
    listChecked.forEach(item => {
      const data = {
        id: 0,
        startTime: moment(this.formControls['startTime'].value).format(DateFormat.YYYY_MM_DD),
        endTime: this.currentlyWorking ? null : moment(this.formControls['endTime'].value).format(DateFormat.YYYY_MM_DD),
        projectId: this.workingExperience.projectId,
        projectName: getFormControlValue(this.form, 'projectName'),
        technologies: getFormControlValue(this.form, 'technologies'),
        position: getFormControlValue(this.form, 'position'),
        projectDescription: getFormControlValue(this.form, 'projectDescription'),
        responsibility: getFormControlValue(this.form, 'responsibility'),
        userId: item.employeeId,
        order: this.workingExperience.order,
        versionId: item.versionId
      };
      promises.push(this._myProfileService.updateWorkingExperience(data));
    });
    
    forkJoin(promises)
      .pipe(
        catchError((err: HttpErrorResponse) => {
          return of({ error: err.error.error });
        })
      )
      .subscribe((res: ApiResponse<WorkingExperience>[]) => {
        if(res.length > 0){
          res[0].success && this.ref.close(res[0]);
        }
      });
  }

  private handleSave(payload: WorkingExperience) {
    this.subs.add(
      this._myProfileService.updateWorkingExperience(payload).subscribe(res => {
        res.success && this.ref.close(res);
      })
    );
  }

  toggleShow() {
    this.currentlyWorking = !this.currentlyWorking;
  }

  setValueCurrentlyWorking() {
    if (this.workingExperience.endTime == null) {
      return this.currentlyWorking = true;
    }
    return this.currentlyWorking = false;
  }


  private initForm() {
    const startTime: Date = this.getTime(this.workingExperience.startTime);
    const endTime: Date = this.getTime(this.workingExperience.endTime);
    this.form = this._fb.group({
      projectName: [
        this.workingExperience.projectName,
        [Validators.required]
      ],
      startTime: [
        startTime,
        [Validators.required]
      ],
      endTime: [
        endTime
      ],
      position: [
        this.workingExperience.position,
        [Validators.required]
      ],
      projectDescription: [
        this.workingExperience.projectDescription,
        [Validators.required]
      ],
      responsibility: [
        this.workingExperience.responsibility,
        [Validators.required]
      ],
      technologies: [
        this.workingExperience.technologies,
        [Validators.required]
      ],
      versionName: [''],
      versionPositionId: [''],
      versionLanguageId: [''],
    });
  }

  private getTime(time: string) {
    return time && new Date(time);
  }
}
