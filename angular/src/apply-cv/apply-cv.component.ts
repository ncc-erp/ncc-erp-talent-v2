import { MESSAGE } from './../shared/AppConsts';
import { ToastMessageType } from './../shared/AppEnums';
import { getFormControlValue, isCVExtensionAllow, isImageExtensionAllow } from '@app/core/helpers/utils.helper';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ApplyCvService } from './apply-cv.service';
import { ApplyCv } from './apply-cv.model';
import { AppComponentBase } from '@shared/app-component-base';
import { Component, OnInit, Injector } from '@angular/core';
import { ActionEnum } from '@shared/AppEnums';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { UploadAvatarComponent } from '@shared/components/upload-avatar/upload-avatar.component';
import { DialogService } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-apply-cv',
  templateUrl: './apply-cv.component.html',
  styleUrls: ['./apply-cv.component.scss']
})
export class ApplyCvComponent extends AppComponentBase implements OnInit {
  
  readonly ACCEPT_IMAGE = '.png, .jpg, .jpeg, .gif';
  readonly ACCEPT_CV = '.doc, .docx, .xlsx, .csv, .pdf';
  readonly DEBOUNCE_TIME = 200; //0.2s
  readonly FORM_DISABLED = 'DISABLED';

  public applyCv: ApplyCv = new ApplyCv();
  id: number;

  action: ActionEnum;
  public form: FormGroup;
  submitted: boolean = false;
  originalFormData: ApplyCv;

  cvFile: File;
  cvUrl: string;
  cvFileName: string;
  showCVFileName: string;
  avatarUrl: string;
  avatarFile: File;
  avatarFileName: string;
  showAvatarFileName: string;
  imageChangedEvent;
  initialFormState: any;
 
  file: any;
  attachCv: File;
  applyCvLink: any;
  postId: number = 0;

  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    public _applyCv: ApplyCvService,
    private _fb: FormBuilder,
    private _dialog: DialogService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.initForm();
    this.route.queryParams.subscribe((queryParams) => {
      if (queryParams['postid']) {
        this.postId = queryParams['postid'];
      }
    })
  }

  get formControls() {
    return this.form.controls;
  }

  onCVFileChange(fileList: FileList) {
    let file = fileList[0];
    this.cvFile = file;
    this.cvFileName = file?.name;
    if (!isCVExtensionAllow(file)) {
      this.formControls['attachCv'].setErrors({ invalidCVExtension: true });
      return;
    }
    for (let i = 0; i < this.cvFileName.length; i++) {
      if (this.cvFileName.length < 20) {
        this.showCVFileName = this.cvFileName;
      }
      else {
        this.showCVFileName = this.cvFileName.slice(0,10) + "..." + this.cvFileName.slice(this.cvFileName.length - 10,this.cvFileName.length)
      }
    }
    const reader = new FileReader();
    reader.readAsText(file);
  }

  onAvatarFileChange(event) {
    this.imageChangedEvent = event;
    const file = event.target.files[0];
    if (!isImageExtensionAllow(file)) {
      this.formControls['avatar'].setErrors({ invalidImageExtension: true });
      return;
    }
    const dialogRef = this._dialog.open(UploadAvatarComponent, {
      header: `Croping image`,
      width: "50%",
      contentStyle: { "max-height": "100%", overflow: "auto" },
      baseZIndex: 10000,
      data: { image: this.imageChangedEvent },
    });

    dialogRef.onClose.subscribe((result: { fileImageString: string, fileImage: File }) => {
      if (!result) return;
      const { fileImageString, fileImage } = result
      this.avatarUrl = fileImageString;
      this.avatarFile = fileImage;
      this.avatarFileName = this.avatarFile.name;
      for (let i = 0; i < this.avatarFileName.length; i++) {
        if (this.avatarFileName.length < 20) {
          this.showAvatarFileName = this.avatarFileName;
        }
        else {
          this.showAvatarFileName =  this.avatarFileName.slice(0,10) + "..." + this.avatarFileName.slice(this.avatarFileName.length - 10,this.avatarFileName.length)
        }
      }
      this.formControls['avatar'].patchValue(this.avatarUrl);
    });
  }

  public initForm() {
    this.form = this._fb.group({
      name: ['', [Validators.required]],
      isFemale: ['', [Validators.required]],
      email:  ['', [Validators.required, Validators.pattern("^[a-z0-9]+(\.[_a-z0-9]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,15})$")]],
      phone: ['', [Validators.required, Validators.pattern("^((\\+91-?)|0)?[0-9]{10}$")]],
      positionType: ['', [Validators.required]],
      jobTitle: ['', [Validators.required]],
      branch: ['', [Validators.required]],
      avatar: null,
      attachCv: [null, []],
    });
    this.initialFormState = this.form.value
  }

  onSubmit() {
    this.submitted = true;
    if (this.form.invalid) return;
    if(!this.cvFile || !this.avatarFile ) return;
    const payload = this.getPayload();
    this._applyCv.create(payload).subscribe(res => {
      this.isLoading = res.loading;
      if (!res.loading && res.success) {
        setTimeout(() =>{
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.APPLY_CV_SUCCESS);
          if (this.form.valid) {
            this.resetForm();
            this.submitted = false
           } 
        }, 1000)
      }     
    })               
  }

  resetForm() {
    this.form.reset(this.initialFormState);
    this.cvFile = null
    this.cvUrl = ''
    this.cvFileName = ''
    this.showAvatarFileName = ''
    this.showCVFileName = ''
    this.avatarUrl = ''
    this.avatarFile = null
    this.avatarFileName = '' 
  };

  getPayload(isUpdate: boolean = false) {
    if(isUpdate) {
      const payload: ApplyCv = {
        name: getFormControlValue(this.form, 'name'),
        isFemale: getFormControlValue(this.form, 'isFemale'),
        email: getFormControlValue(this.form, 'email'),
        phone: getFormControlValue(this.form, 'phone'),
        positionType: getFormControlValue(this.form, 'positionType'),
        jobTitle: getFormControlValue(this.form, 'jobTitle'),
        branch: getFormControlValue(this.form, 'branch'),
        avatar: getFormControlValue(this.form, 'avatar'),
        attachCv: getFormControlValue(this.form, 'attachCv'),
        postId: this.postId
      };
      return payload;
    }

    const formData = new FormData();
    formData.append('name', getFormControlValue(this.form, 'name'));
    formData.append('isFemale', getFormControlValue(this.form, 'isFemale'));
    formData.append('email', getFormControlValue(this.form, 'email'));
    formData.append('phone', getFormControlValue(this.form, 'phone'));
    formData.append('positionType', getFormControlValue(this.form, 'positionType'));
    formData.append('jobTitle', getFormControlValue(this.form, 'jobTitle'));
    formData.append('branch', getFormControlValue(this.form, 'branch'));

    this.cvFile && formData.append('attachCv', this.cvFile);
    this.avatarFile && formData.append('avatar', this.avatarFile);
    this.postId && formData.append('postId', '' + this.postId);
    return formData;
  }
  
  toAbout(){
    document.getElementById("about-us").scrollIntoView();
  }

  joinUsForm() {
    document.getElementById("recruitement-contact-form").scrollIntoView();
  }
}
