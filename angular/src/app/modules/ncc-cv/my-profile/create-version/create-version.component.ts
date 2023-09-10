import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { JobPosition } from '@app/core/models/categories/job-position.model';
import { ProfileVersion } from '@app/core/models/employee-profile/profile-model';
import { VersionService } from '@app/core/services/employee-profile/version.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { MESSAGE } from '@shared/AppConsts';
import { ToastMessageType } from '@shared/AppEnums';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-create-version',
  templateUrl: './create-version.component.html',
  styleUrls: ['./create-version.component.scss']
})
export class CreateVersionComponent extends AppComponentBase implements OnInit {

  form: FormGroup;
  position: JobPosition[] = [];
  language: JobPosition[] = [];
  submitted = false;

  constructor(
    injector: Injector,
    public config: DynamicDialogConfig,
    public dialogRef: DynamicDialogRef,
    public _utilities: UtilitiesService,
    private fb: FormBuilder,
    private _version: VersionService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.buildForm();
  }

  get formControls() { return this.form.controls; }

  buildForm() {
    this.form = this.fb.group({
      nameVersion: ['', Validators.required],
      position: ['', Validators.required],
      language: [''],
    });
  }

  submit() {
    this.submitted = true;
    if (this.form.invalid) return;

    const formData = this.form.value;
    const data: ProfileVersion = {
      employeeId: this.config.data,
      versionName: formData.nameVersion,
      positionId: formData.position,
      languageId: formData.language,
      id: 0
    };


    this._version.createVersion(data).subscribe(res => {
      this.isLoading = res.loading;
      res.success && this.dialogRef.close(res);
    })
  }
}