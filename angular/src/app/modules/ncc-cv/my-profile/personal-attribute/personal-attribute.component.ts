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
  attributeForm: FormGroup;
  listData;
  foundItem: number;
  isSale: boolean;
  isUser: boolean;
  id: number;
  submitted = false;
  public listInputValid = [{ text: 'attribute', value: false }]

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
    this.listData = (!this.data.listPersonalAttribute || this.data?.listPersonalAttribute?.length === 0)  ? [] : this.data.listPersonalAttribute;

    if (!this.data.item) {
      return;
    }

    for (let i = 0; i <= this.listData.length; i++) {
      if (this.data.item === this.listData[i]) {
        this.foundItem = i;
      }
    }
    this.attributeForm.get('attribute').setValue(this.config.data.item);
  }

  get formControls() { return this.attributeForm.controls; }


  saveAttribute() {
    this.submitted = true;
    if(this.attributeForm.invalid) return;

    if (!this.listInputValid[0].value && this.attributeForm.controls.attribute.value) {
      if (this.action === ActionEnum.UPDATE) {
        for (let i = 0; i <= this.listData.length; i++) {
          if (this.foundItem === i) {
            this.listData[i] = this.attributeForm.controls.attribute.value;
          }
        }
      } else {
        this.listData.push(this.attributeForm.controls.attribute.value);
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
    this.attributeForm = this.fb.group({
      attribute: [null, [Validators.required]],
    });
  }

}

