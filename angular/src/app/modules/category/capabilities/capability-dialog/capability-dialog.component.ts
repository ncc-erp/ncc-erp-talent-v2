import { ToastMessageType } from '@shared/AppEnums';
import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Capability, CapabilityConfigDiaLog } from '@app/core/models/categories/capabilities.model';
import { CapabilityService } from '@app/core/services/categories/capability.service';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { AppComponentBase } from '../../../../../shared/app-component-base';
import { ActionEnum } from '../../../../../shared/AppEnums';
import { MESSAGE } from '@shared/AppConsts';

@Component({
  selector: 'talent-capability-dialog',
  templateUrl: './capability-dialog.component.html',
  styleUrls: ['./capability-dialog.component.scss']
})
export class CapabilityDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;
  capabilityConfigDialog: CapabilityConfigDiaLog;
  action: ActionEnum;

  constructor(
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private injector: Injector,
    private _fb: FormBuilder,
    private _capability: CapabilityService
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.capabilityConfigDialog = this.config.data;
    this.action = this.capabilityConfigDialog.action;
    this.initForm();
  }

  onSave(isClose: boolean) {
    this.submitted = true;
    if (this.form.invalid) return;
    
    const action = this.capabilityConfigDialog.action;
    const payload: Capability = {
      id: action === ActionEnum.UPDATE ? this.capabilityConfigDialog.capability.id : 0,
      name: this.formControls['name'].value,
      note: this.formControls['note'].value,
      fromType: this.formControls['isHr'].value,
    };
    this.handleSave(payload, action, isClose);
  }

  checkAndClosePopup(res: ApiResponse<Capability>) {
    if (!res.loading) this.ref.close(res);
  }

  private initForm() {
    this.form = this._fb.group({
      name: [
        this.capabilityConfigDialog.action == ActionEnum.UPDATE ? this.capabilityConfigDialog.capability.name : '',
        [Validators.required]
      ],
      note: [ 
        this.capabilityConfigDialog.action == ActionEnum.UPDATE ? this.capabilityConfigDialog.capability.note : '',
        [Validators.maxLength(5000)]
      ],
      isHr: this.capabilityConfigDialog.action == ActionEnum.UPDATE ? this.capabilityConfigDialog.capability.fromType : false,
    });
  }

  private resetForm() {
    this.initForm();
    this.submitted = false;
  }

  private handleSave(payload: Capability, action: ActionEnum, isClose: boolean) {
    if (action === ActionEnum.CREATE) {
      this.subs.add(
        this._capability.create(payload).subscribe(res =>  {
          this.isLoading = res.loading;
          if (res.loading) return;
          this.doSave(res,isClose);
        })
      );
      return;
    }

    this.subs.add(
      this._capability.update(payload).subscribe(res => {
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
