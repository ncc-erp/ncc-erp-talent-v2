import { Component, Injector, OnInit } from '@angular/core';
import { MailPreviewInfo } from '@app/core/models/mail/mail.model';
import { MailService } from '@app/core/services/apis/mail.service';
import { AppComponentBase } from '@shared/app-component-base';
import { MAIL_TYPE, ToastMessageType } from '@shared/AppEnums';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { EditMailDialogComponent } from '../edit-mail-dialog/edit-mail-dialog.component';

@Component({
  selector: 'talent-mail-dialog',
  templateUrl: './mail-dialog.component.html',
  styleUrls: ['./mail-dialog.component.scss'],
})
export class MailDialogComponent extends AppComponentBase implements OnInit {
  mailInfo: MailPreviewInfo = new MailPreviewInfo();
  templateId: number;
  isAllowSendMail: boolean = true;
  isSending: boolean = false;
  showEditBtn = false;

  constructor(
    private injector: Injector,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private _mail: MailService,
    private _dialog: DialogService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    Object.assign(this, this.config.data);
    if (this.templateId) this.getTemplateById();
  }

  getTemplateById() {
    this.subs.add(
      this._mail.getByIdFakeData(this.templateId).subscribe((res: ApiResponse<MailPreviewInfo>) => {
        if (res?.success) this.mailInfo = res.result;
      })
    )
  }

  editMail() {
    const dialogRef = this._dialog.open(EditMailDialogComponent, {
      showHeader: false,
      width: '90%',
      height: "80vh",
      contentStyle: { 'background-color': 'rgba(242,245,245)', overflow: 'visible' },
      baseZIndex: 10000,
      data: { mailInfo: this.mailInfo }
    });
    dialogRef.onClose.subscribe((res: MailPreviewInfo) => res && (this.mailInfo = res));
  }

  sendMail() {
    this.confirmationService.confirm({
      message: 'Are you sure that you want send email?',
      header: 'Confirmation',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        if (!this.mailInfo.to) {
          this.showToastMessage(ToastMessageType.ERROR, 'Email Invalid');
          return;
        }

        if (this.ref) this.ref.close(this.mailInfo);
      },
    });
  }

  cancel() { }

  checkAndClosePopup(res) {
    if (!res.loading) this.ref.close(res);
  }
}
