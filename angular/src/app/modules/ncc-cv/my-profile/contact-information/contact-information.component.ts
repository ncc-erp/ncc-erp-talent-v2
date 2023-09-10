import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { getFormControlValue } from '@app/core/helpers/utils.helper';
import { ContactInformation, IObjectFile } from '@app/core/models/employee-profile/profile-model';
import { MyProfileService } from '@app/core/services/employee-profile/my-profile.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { AppConsts, MESSAGE } from '@shared/AppConsts';
import { ToastMessageType } from '@shared/AppEnums';
import { AppSessionService } from '@shared/session/app-session.service';
import { PermissionCheckerService } from 'abp-ng2-module';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-contact-information',
  templateUrl: './contact-information.component.html',
  styleUrls: ['./contact-information.component.scss']
})
export class ContactInformationComponent extends AppComponentBase implements OnInit {
  contactInfo: ContactInformation;

  imagePath: any;
  img = {
    buffer: null,
    file: null,
    path: '',
  } as IObjectFile;
  
  surname: string;
  name: string;
  file: File = null;
  showImg = true;
  urlImg = AppConsts.remoteServiceBaseUrl;
  isSale: boolean;
  isUser: boolean;
  isEmployee = false;
  id: number;

  submitted = false;
  form: FormGroup;

  constructor(
    injector: Injector,
    public config: DynamicDialogConfig,
    public dialogRef: DynamicDialogRef,
    public _utilities: UtilitiesService,
    private _myProfile: MyProfileService,
    private _permissionChecker: PermissionCheckerService,
    private session: AppSessionService,
    private _fb: FormBuilder,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.isSale = this._permissionChecker.isGranted('Pages.EditAsSales.Employee');
    this.contactInfo = this.config.data;
    this.isUser = this.session.userId === Number(this.contactInfo.userId);
    this.initForm();
  }

  get formControls() {
    return this.form.controls;
  }

  changeInmage(event: any): void {
    this.showImg = false;
    const reader = new FileReader();
    this.img.file = event.target.files[0];
    reader.readAsDataURL(this.img.file);
    reader.onload = (_event) => {
      this.img.buffer = reader.result;
    };
  }

  deleteImage() {
    this.img.file = null;
    this.contactInfo.imgPath = '';
  }
  
  submitInfoContact() {
    this.submitted = true;
    if (this.form.invalid) return;
    if (this.isSale && !this.isUser) this.dialogRef.close(this.form.getRawValue());

    const request = new FormData();

    if (this.img.file) {
      request.append('File', this.img.file);
    } else {
      request.append('Path', this.contactInfo.imgPath);
      request.append('File', '');
    }
    request.append('Name', getFormControlValue(this.form, 'name'));
    request.append('Surname', getFormControlValue(this.form, 'surname'));
    request.append('PhoneNumber', getFormControlValue(this.form, 'phoneNumber'));
    request.append('EmailAddressInCV', getFormControlValue(this.form, 'emailAddressInCV'));
    request.append('CurrentPositionId', getFormControlValue(this.form, 'currentPositionId'));
    this.contactInfo.userId && request.append('UserId', this.contactInfo.userId);
    request.append('Address', getFormControlValue(this.form, 'address'));
    request.append('BranchId', getFormControlValue(this.form, 'branchId'));

    this._myProfile.saveUserGeneralInfo(request).subscribe(res => {
      this.isLoading = res.loading;
      if (res.loading) return;

      this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);
      this.dialogRef.close(res);
    });
  }

  private initForm() {
    const name = this.contactInfo?.name ? this.contactInfo.name : '';
    const surname = this.contactInfo?.surname ? this.contactInfo.surname : '';
    const phoneNumber = this.contactInfo?.phoneNumber ? this.contactInfo.phoneNumber : null;
    const emailAddressInCV = this.contactInfo?.emailAddressInCV ? this.contactInfo.emailAddressInCV : '';
    const imgPath = this.contactInfo?.imgPath ? this.contactInfo.imgPath : '';
    const currentPositionId = this.contactInfo?.currentPositionId ? this.contactInfo.currentPositionId : null;
    const userId = this.contactInfo?.userId ? this.contactInfo.userId : null;
    const address = this.contactInfo?.address ? this.contactInfo.address : '';
    const branchId = this.contactInfo?.branchId ? this.contactInfo.branchId : null;

    this.form = this._fb.group({
      name: [name, [Validators.required]],
      surname: [surname, [Validators.required]],
      phoneNumber: [phoneNumber, [Validators.required]],
      emailAddressInCV: [emailAddressInCV, [Validators.required, Validators.email]],
      imgPath: imgPath,
      currentPositionId: [currentPositionId, [Validators.required]],
      userId: [userId, [Validators.required]],
      address: [address, [Validators.required]],
      branchId: [branchId, [Validators.required]],
    });
  }
}

