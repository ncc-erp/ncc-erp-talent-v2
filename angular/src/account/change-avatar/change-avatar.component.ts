import { Component, Injector, OnInit} from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { isImageExtensionAllow } from '@app/core/helpers/utils.helper';
import { MESSAGE } from '@shared/AppConsts';
import { ToastMessageType } from '@shared/AppEnums';
import { AppComponentBase } from '@shared/app-component-base';
import { AvatarDto, UserServiceProxy } from '@shared/service-proxies/service-proxies';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-change-avatar',
  templateUrl: './change-avatar.component.html',
  styleUrls: ['./change-avatar.component.scss']
})
export class ChangeAvatarComponent extends AppComponentBase implements OnInit{

  readonly ACCEPT_IMAGE = '.png, .jpg, .jpeg, .gif';

  form: FormGroup;
  imageChangedEvent: any;
  showCropper = false;
  avatarUrl: string;
  croppedImage: any;
  file = null;
  fileIamgeName: any;

  status: any;

  constructor(
    injector: Injector,
    public dialogRef: DynamicDialogRef,
    private _userService: UserServiceProxy,
  ) {
    super(injector);
    this.form = new FormGroup({
      avatar: new FormControl()
    });
  }

  ngOnInit(): void {
  }
  get formControls() {
    return this.form.controls;
  }

  onAvatarFileChange(event) {
    if(event.target.files.length != 0){
      this.imageChangedEvent = event;
      this.file = event.target.files[0];
      this.fileIamgeName = this.file?.name;
      if (this.file != null && !isImageExtensionAllow(this.file)) {
        this.formControls['avatar'].setErrors({ invalidImageExtension: true });
        return;
      }
    }
  }

  imageCropped(event: ImageCroppedEvent) {
    this.croppedImage = event.base64;
  }

  imageLoaded() {
    this.showCropper = true;
  }

  cropperReady() {
  }

  loadImageFailed() {
  }

  onSave() {
    this.file = new File([this.dataURItoBlob(this.croppedImage)], this.fileIamgeName);
    this.uploadFile(this.file);
    this.dialogRef.close()
  }

  onClose(){
    this.dialogRef.close();
  }

  private dataURItoBlob(dataURI): Blob {
    const byteString = atob(dataURI.split(',')[1]);
    const ab = new ArrayBuffer(byteString.length);
    let ia = new Uint8Array(ab);
    for (let i = 0; i < byteString.length; i++) {
      ia[i] = byteString.charCodeAt(i);
    }
    return new Blob([ab], { type: 'image/jpeg' });
  }

  private uploadFile(file: File) {
    this._userService.upLoadOwnAvatar(file)
    .subscribe(data => {
        if (data) {
          let user: AvatarDto = data;
          this.appSession.user.avatarPath = user.avatarPath
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);
        } else {
          this.showToastMessage(ToastMessageType.ERROR, MESSAGE.UPDATE_FAILED);
        }
    });
  }
}
