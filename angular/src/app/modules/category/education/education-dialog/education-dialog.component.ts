import { UtilitiesService } from './../../../../core/services/utilities.service';
import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EducationType, EducationTypeConfigDiaLog } from '@app/core/models/categories/education-type.model';
import { EducationService } from '@app/core/services/categories/education.service';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ActionEnum } from '../../../../../shared/AppEnums';
import { ApiResponse } from '../../../../../shared/paged-listing-component-base';
import { ToastMessageType } from './../../../../../shared/AppEnums';
import { Education, EducationConfigDiaLog } from './../../../../core/models/categories/education.model';
import { EducationTypesDialogComponent } from './../../education-types/education-types-dialog/education-types-dialog.component';
import { MESSAGE } from '@shared/AppConsts';
import { getFormControlValue, randomHexColor } from '@app/core/helpers/utils.helper';

@Component({
  selector: 'talent-education-dialog',
  templateUrl: './education-dialog.component.html',
  styleUrls: ['./education-dialog.component.scss']
})
export class EducationDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  eduDialogConfig: EducationConfigDiaLog;
  action: ActionEnum;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    public _utilities: UtilitiesService,
    public dialogService: DialogService,
    private injector: Injector,
    private _fb: FormBuilder,
    private _education: EducationService,
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.eduDialogConfig = this.config.data;
    this.action = this.eduDialogConfig.action;
    this.initForm();
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const action = this.eduDialogConfig.action;
    const payload: Education = {
      id: action === ActionEnum.UPDATE ? this.eduDialogConfig.education.id : 0,
      educationTypeId: this.formControls['educationTypeId'].value,
      name: this.formControls['name'].value,
      colorCode: getFormControlValue(this.form, 'colorCode'),
    };
    this.handleSave(payload, action, isClose);
  }

  openEducationTypeDialog() {
    const dialogConfig: EducationTypeConfigDiaLog = { educationType: null, action: ActionEnum.CREATE };
    const dialogRef = this.dialogService.open(EducationTypesDialogComponent, {
      header: `${dialogConfig.action} Education Type`,
      width: "40%",
      contentStyle: { "max-height": "500px", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<EducationType>) => {
      this._utilities.loadCatalogForCategories();
      if (res) {
        const newEduType: EducationType = res.result;
        this.formControls['educationTypeId'].setValue(newEduType.id);
      }
    });
  }

  checkAndClosePopup(res: ApiResponse<Education>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    const isUpdate = this.eduDialogConfig.action == ActionEnum.UPDATE;
    const hexColor = isUpdate ? this.eduDialogConfig.education.colorCode : randomHexColor();
    this.form = this._fb.group({
      name: [isUpdate ? this.eduDialogConfig.education.name : '', [Validators.required]],
      educationTypeId: [isUpdate ? this.eduDialogConfig.education.educationTypeId : '', [Validators.required]],
      colorCode: [hexColor, [Validators.required]],
      colorCodeIp: [hexColor, [Validators.required]]
    });

    this.formControls.colorCodeIp.valueChanges.subscribe(color => {
      if (!color.startsWith('#')) color = `#${color}`
      this.formControls.colorCode.patchValue(color, { emitEvent: false});
    })

    this.formControls.colorCode.valueChanges.subscribe(color => {
      this.formControls.colorCodeIp.patchValue(color, { emitEvent: false});
    })
  }

  private resetForm() {
    this.form.reset({ ...this.form.value, name: '' })
    this.submitted = false;
  }

  private handleSave(payload: Education, action: ActionEnum, isClose: boolean) {
    if (action === ActionEnum.CREATE) {
      this.subs.add(
        this._education.create(payload).subscribe(res => {
          this.isLoading = res.loading;
          if (res.loading) return;
          this.doSave(res, isClose);
        })
      );
      return;
    }

    this.subs.add(
      this._education.update(payload).subscribe(res => this.checkAndClosePopup(res))
    );
  }

  private doSave(res, isClose: boolean) {
    this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS, res.result.displayName);
    if (res.success && isClose) {
      this.checkAndClosePopup(res);
      return;
    }
    this.resetForm();
  }
}
