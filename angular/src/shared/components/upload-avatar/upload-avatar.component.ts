import { Component, OnInit, Optional, ViewChild } from '@angular/core';
import { ImageCroppedEvent, ImageCropperComponent } from 'ngx-image-cropper';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-upload-avatar',
  templateUrl: './upload-avatar.component.html',
  styleUrls: ['./upload-avatar.component.scss']
})
export class UploadAvatarComponent implements OnInit {

  imageChangedEvent: any;
  croppedImage: string = '';
  showCropper = false;

  avatarFile: File;

  @ViewChild(ImageCropperComponent) imageCropper: ImageCropperComponent;

  constructor(
    @Optional() public config: DynamicDialogConfig,
    @Optional() public ref: DynamicDialogRef) {
  }

  ngOnInit(): void {
    this.imageChangedEvent = this.config.data?.image;
  }

  imageCropped(event: ImageCroppedEvent) {
    this.croppedImage = event.base64;
  }

  imageLoaded() {
    this.showCropper = true;
  }

  onSave() {
    const file = new File([this.dataURItoBlob(this.croppedImage)], "image.jpg");
    this.ref.close({ fileImageString: this.croppedImage, fileImage: file })
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
}
