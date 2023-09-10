import { ToastMessageType } from './../../../../../shared/AppEnums';
import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue } from '@app/core/helpers/utils.helper';
import { ProjectService } from '@app/core/services/project/project.service';
import { AppComponentBase } from '@shared/app-component-base';
import { ActionEnum } from '@shared/AppEnums';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { Project } from '../list-project/list-project.component';

@Component({
  selector: 'talent-dialog-project',
  templateUrl: './dialog-project.component.html',
  styleUrls: ['./dialog-project.component.scss']
})
export class DialogProjectComponent extends AppComponentBase implements OnInit {
  submitted: boolean = false;
  form: FormGroup;
  isSaving: boolean = false;
  action: ActionEnum;
  project: Project;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private injector: Injector,
    private _fb: FormBuilder,
    private _project: ProjectService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getDialogConfig();
    this.initForm();
  }

  private getDialogConfig() {
    this.action = this.config.data.action;
    this.project = this.config.data.project;
  }

  get formControls() {
    return this.form.controls;
  }

  onSave() {
    this.submitted = true;
    if (this.form.invalid) return;
    const payload = {
      id: this.action == ActionEnum.CREATE ? 0 : this.project.id,
      name: getFormControlValue(this.form, 'name'),
      technology: getFormControlValue(this.form, 'technology'),
      description: getFormControlValue(this.form, 'description')
    } as Project;

    this.handleSave(payload);
  }

  private handleSave(payload: Project) {
    this.subs.add(
      this._project.save(payload).subscribe(res => {
        if (res.error) {
          this.showToastMessage(ToastMessageType.ERROR, `${this.action} Failed`, res.error.message);
          return;
        }
        res.success && this.closePopUp(res);
      })
    );
  }

  private closePopUp(res: ApiResponse<Project>) {
    this.isSaving = res.loading;
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    const isUpdate = this.action == ActionEnum.UPDATE;
    this.form = this._fb.group({
      name: [
        isUpdate ? this.project.name : '',
        [Validators.required]
      ],
      technology: [
        isUpdate ? this.project.technology : '',
        [Validators.required]
      ],
      description: [
        isUpdate ? this.project.description : '',
        [Validators.required]
      ]
    });
  }

}

export class ProjectDialogConfig {
  action: ActionEnum;
  project: Project;
}
