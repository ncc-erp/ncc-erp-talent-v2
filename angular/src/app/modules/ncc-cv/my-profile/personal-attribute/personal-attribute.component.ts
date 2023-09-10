import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PersonalAttributeConfigDialog } from '@app/core/models/employee-profile/profile-model';
import { MyProfileService } from '@app/core/services/employee-profile/my-profile.service';
import { AppComponentBase } from '@shared/app-component-base';
import { ActionEnum, ToastMessageType } from '@shared/AppEnums';
import { PermissionCheckerService } from 'abp-ng2-module';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { enterName } from '../../employee-profile.helper';

@Component({
  selector: 'talent-personal-attribute',
  templateUrl: './personal-attribute.component.html',
  styleUrls: ['./personal-attribute.component.scss']
})
export class PersonalAttributeComponent extends AppComponentBase implements OnInit {
  atributeForm: FormGroup;
  listData;
  foundItem: number;
  isSale: boolean;
  isUser: boolean;
  id: number;
  submitted = false;
  public listInputValid = [{ text: 'atribute', value: false }]

  data: PersonalAttributeConfigDialog;
  action: ActionEnum;
  constructor(
    injector: Injector,
    private fb: FormBuilder,
    private _myProfile: MyProfileService,
    public config: DynamicDialogConfig,
    public dialogRef: DynamicDialogRef,
    private _permissionChecker: PermissionCheckerService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.data = this.config.data;
    this.isSale = this._permissionChecker.isGranted('Pages.EditAsSales.Employee');
    this.isUser = this.data.isUser;
    this.action = this.data.action;

    this.buildForm();
    this.listData = (!this.data.listPersonalAtribute || this.data?.listPersonalAtribute?.length === 0)  ? [] : this.data.listPersonalAtribute;

    if (!this.data.item) {
      return;
    }

    for (let i = 0; i <= this.listData.length; i++) {
      if (this.data.item === this.listData[i]) {
        this.foundItem = i;
      }
    }
    this.atributeForm.get('atribute').setValue(this.config.data.item);
  }

  get formControls() { return this.atributeForm.controls; }


  saveAtribute() {
    this.submitted = true;
    if(this.atributeForm.invalid) return;
    
    if (!this.listInputValid[0].value && this.atributeForm.controls.atribute.value) {
      if (this.action === ActionEnum.UPDATE) {
        for (let i = 0; i <= this.listData.length; i++) {
          if (this.foundItem === i) {
            this.listData[i] = this.atributeForm.controls.atribute.value;
          }
        }
      } else {
        this.listData.push(this.atributeForm.controls.atribute.value);
      }
      const data = {
        personalAttributes: this.listData
      };
      if (this.isSale && !this.isUser) {
        this.dialogRef.close(data);
      } else {
        this._myProfile.updatePersonalAttribute(data).subscribe(res => {
          this.isLoading = res.loading;
          res.success && this.dialogRef.close(res);
        });
      }
    }
    else {
      this.notify.error("Bạn chưa nhập thông tin!");
    }
  }

  onValueChange(value, index) {
    this.listInputValid[index].value = enterName(value);
  }

  private buildForm() {
    this.atributeForm = this.fb.group({
      atribute: [null, [Validators.required]],
    });
  }

}

