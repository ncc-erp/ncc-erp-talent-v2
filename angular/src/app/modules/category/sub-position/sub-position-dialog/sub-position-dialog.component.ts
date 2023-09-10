import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue, randomHexColor } from '@app/core/helpers/utils.helper';
import { JobPosition, JobPositionConfigDiaLog } from '@app/core/models/categories/job-position.model';
import { SubPositionConfigDiaLog } from '@app/core/models/categories/sub-position.model';
import { SubPositionService } from '@app/core/services/categories/sub-position.service';
import { MESSAGE } from '@shared/AppConsts';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ActionEnum } from '../../../../../shared/AppEnums';
import { ApiResponse } from '../../../../../shared/paged-listing-component-base';
import { JobPositionDialogComponent } from '../../job-positions/job-position-dialog/job-position-dialog.component';
import { ToastMessageType } from './../../../../../shared/AppEnums';
import { SubPosition } from './../../../../core/models/categories/sub-position.model';
import { UtilitiesService } from './../../../../core/services/utilities.service';

@Component({
  selector: 'talent-sub-position-dialog',
  templateUrl: './sub-position-dialog.component.html',
  styleUrls: ['./sub-position-dialog.component.scss']
})
export class SubPositionDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  subPositionDialogConfig: SubPositionConfigDiaLog;
  action: ActionEnum;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    public _utilities: UtilitiesService,
    public dialogService: DialogService,
    private injector: Injector,
    private _fb: FormBuilder,
    private _subPosition: SubPositionService,
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.subPositionDialogConfig = this.config.data;
    this.action = this.subPositionDialogConfig.action;
    this.initForm();
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const action = this.subPositionDialogConfig.action;
    const payload: SubPosition = {
      id: action === ActionEnum.UPDATE ? this.subPositionDialogConfig.subPosition.id : 0,
      name: getFormControlValue(this.form, 'name'),
      colorCode: getFormControlValue(this.form, 'colorCode'),
      positionId: getFormControlValue(this.form, 'positionId'),
    };
    this.handleSave(payload, action, isClose);
  }

  openMainPositionDialog() {
    const dialogConfig: JobPositionConfigDiaLog = { jobPosition: null, action: ActionEnum.CREATE, showButtonSave: false };
    const dialogRef = this.dialogService.open(JobPositionDialogComponent, {
      header: `${dialogConfig.action} Position`,
      width: "40%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<JobPosition>) => {
      this._utilities.loadCatalogForCategories();
      if (res) {
        const newPosition: JobPosition = res.result;
        this.formControls['positionId'].setValue(newPosition.id);
      }
    });
  }

  checkAndClosePopup(res: ApiResponse<SubPosition>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    const isUpdate = this.subPositionDialogConfig.action === ActionEnum.UPDATE;
    const hexColor = isUpdate ? this.subPositionDialogConfig.subPosition.colorCode : randomHexColor();

    this.form = this._fb.group({
      name: [
        isUpdate ? this.subPositionDialogConfig.subPosition.name : '',
        [Validators.required]
      ],
      positionId: [
        isUpdate ? this.subPositionDialogConfig.subPosition.positionId : '',
        [Validators.required]
      ],
      colorCode: [hexColor, [Validators.required]],
      colorCodeIp: [hexColor, [Validators.required]]
    });

    this.formControls.colorCodeIp.valueChanges.subscribe(color => {
      if (!color.startsWith('#')) color = `#${color}`
      this.formControls.colorCode.patchValue(color, { emitEvent: false });
    })

    this.formControls.colorCode.valueChanges.subscribe(color => {
      this.formControls.colorCodeIp.patchValue(color, { emitEvent: false });
    })
  }

  private resetForm() {
    this.form.reset({
      ...this.form.value,
      name: '',
      color: randomHexColor(),
      colorCodeIp: randomHexColor()
    })
    this.submitted = false;
  }

  private handleSave(payload: SubPosition, action: ActionEnum, isClose: boolean) {
    if (action === ActionEnum.CREATE) {
      this.subs.add(
        this._subPosition.create(payload).subscribe(res => {
          this.isLoading = res.loading;
          if (res.loading) return;
          this.doSave(res, isClose);
        })
      );
      return;
    }

    this.subs.add(
      this._subPosition.update(payload).subscribe(res => {
        this.isLoading = res.loading;
        this.checkAndClosePopup(res)
      })
    );
  }

  private doSave(res, isClose: boolean) {
    this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS, res.result.name);
    if (res.success && isClose) {
      this.checkAndClosePopup(res);
      return;
    }
    this.resetForm();
  }

}
