import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue, randomHexColor } from '@app/core/helpers/utils.helper';
import { CVSource, CVSourceConfigDiaLog } from '@app/core/models/categories/cv-source.model';
import { CvSourceService } from '@app/core/services/categories/cv-source.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { MESSAGE } from '@shared/AppConsts';
import { ToastMessageType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ActionEnum } from '../../../../../shared/AppEnums';

@Component({
  selector: 'talent-cv-source-dialog',
  templateUrl: './cv-source-dialog.component.html',
  styleUrls: ['./cv-source-dialog.component.scss']
})
export class CvSourceDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  cvSourceConfigDialog: CVSourceConfigDiaLog;
  action: ActionEnum;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    public _utilities: UtilitiesService,
    private injector: Injector,
    private _fb: FormBuilder,
    private _cvSource: CvSourceService
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.cvSourceConfigDialog = this.config.data;
    this.action = this.cvSourceConfigDialog.action;
    this.initForm();
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const action = this.cvSourceConfigDialog.action;
    const payload: CVSource = {
      id: action === ActionEnum.UPDATE ? this.cvSourceConfigDialog.cvSource.id : 0,
      name: getFormControlValue(this.form, 'name'),
      colorCode: getFormControlValue(this.form, 'colorCode'),
      referenceType: getFormControlValue(this.form, 'referenceType')
    };
    this.handleSave(payload, action, isClose);
  }

  checkAndClosePopup(res: ApiResponse<CVSource>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    const isUpdate = this.cvSourceConfigDialog.action == ActionEnum.UPDATE;
    const hexColor = isUpdate ? this.cvSourceConfigDialog.cvSource.colorCode : randomHexColor();

    this.form = this._fb.group({
      name: [
        isUpdate ? this.cvSourceConfigDialog.cvSource.name : '',
        [Validators.required]
      ],
      referenceType: [isUpdate ? this.cvSourceConfigDialog.cvSource.referenceType : null],
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
    this.initForm();
    this.submitted = false;
  }

  private handleSave(payload: CVSource, action: ActionEnum, isClose: boolean) {
    if (action === ActionEnum.CREATE) {
      this.subs.add(
        this._cvSource.create(payload).subscribe(res => {
          this.isLoading = res.loading;
          if (res.loading) return;
          this.doSave(res,isClose);
        })
      );
      return;
    }

    this.subs.add(
      this._cvSource.update(payload).subscribe(res => {
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
