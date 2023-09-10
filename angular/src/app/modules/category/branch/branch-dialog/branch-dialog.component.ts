import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue, randomHexColor } from '@app/core/helpers/utils.helper';
import { Branch, BranchConfigDiaLog } from '@app/core/models/categories/branch.model';
import { BranchService } from '@app/core/services/categories/branch.service';
import { MESSAGE } from '@shared/AppConsts';
import { ToastMessageType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ActionEnum } from '../../../../../shared/AppEnums';

@Component({
  selector: 'talent-branch-dialog',
  templateUrl: './branch-dialog.component.html',
  styleUrls: ['./branch-dialog.component.scss']
})
export class BranchDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  branchConfigDialog: BranchConfigDiaLog;
  action: ActionEnum;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private injector: Injector,
    private _fb: FormBuilder,
    private _branch: BranchService
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.branchConfigDialog = this.config.data;
    this.action = this.branchConfigDialog.action;
    this.initForm();
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    const action = this.branchConfigDialog.action;
    const payload: Branch = {
      id: action === ActionEnum.UPDATE ? this.branchConfigDialog.branch.id : 0,
      name: getFormControlValue(this.form, 'name'),
      colorCode: getFormControlValue(this.form, 'colorCode'),
      displayName: getFormControlValue(this.form, 'displayName'),
      address: getFormControlValue(this.form, 'address')
    };
    this.handleSave(payload, action, isClose);
  }

  checkAndClosePopup(res: ApiResponse<Branch>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    const isUpdate = this.branchConfigDialog.action == ActionEnum.UPDATE;
    const hexColor = isUpdate ? this.branchConfigDialog.branch.colorCode : randomHexColor();
    this.form = this._fb.group({
      name: [
        isUpdate ? this.branchConfigDialog.branch.name : '',
        [Validators.required]
      ],
      displayName: [
        isUpdate ? this.branchConfigDialog.branch.displayName : '',
        [Validators.required]
      ],
      address: [
        isUpdate ? this.branchConfigDialog.branch.address : '',
        [Validators.required]
      ],
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

  eventHandler(event) {
    if((event.keyCode === 9 || event.keyCode === 13) && !getFormControlValue(this.form, 'displayName')){
      this.formControls.displayName.patchValue(event.target.value);
    }
  }

  private resetForm() {
    this.initForm();
    this.submitted = false;
  }

  private handleSave(payload: Branch, action: ActionEnum, isClose: boolean) {
    if (action === ActionEnum.CREATE) {
      this.subs.add(
        this._branch.create(payload).subscribe(res => {
          this.isLoading = res.loading;
          if (res.loading) return;
          this.doSave(res,isClose);
        })
      );
      return;
    }

    this.subs.add(
      this._branch.update(payload).subscribe(res => {
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

