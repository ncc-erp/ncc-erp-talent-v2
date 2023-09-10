import { MailPreviewInfo } from '@app/core/models/mail/mail.model';
import { EditMailDialogComponent } from './edit-mail-dialog/edit-mail-dialog.component';
import { MailDialogComponent } from './mail-dialog/mail-dialog.component';
import { Mail } from './../../../core/models/mail/mail.model';
import { MailService } from './../../../core/services/apis/mail.service';
import { Router } from '@angular/router';
import { Component, OnInit, OnDestroy, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { LazyLoadEvent } from 'primeng/api';
import { DefaultRoute, ToastMessageType } from '@shared/AppEnums';

@Component({
  selector: 'talent-mail',
  templateUrl: './mail.component.html',
  styleUrls: ['./mail.component.scss']
})
export class MailComponent extends AppComponentBase implements OnInit, OnDestroy {
  private dialogRef: DynamicDialogRef;
  public mails: Mail[] = [];
  public isLoading: boolean;
  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _mail: MailService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
    this.refresh();
  }

  lazyLoadingData(event: LazyLoadEvent) {
    this.refresh();
  }

  refresh() {
    this._mail.getAll().subscribe((rs) => {
      if (rs.success) {
        this.mails = rs.result;
      }
      this.isLoading = rs.loading;
    })
  }

  preview(mailData: Mail) {
    const dialogRef = this.dialogService.open(MailDialogComponent, {
      header: `Preview ${mailData.name} Template`,
      width: '70%',
      contentStyle: { 'max-height': '600px', 'background-color': 'rgba(242,245,245)', overflow: 'auto' },
      baseZIndex: 10000,
      data: { templateId: mailData.id, isAllowSendMail: this.permission.isGranted(this.PS.Pages_Mails_SendMail) }
    });

    dialogRef.onClose.subscribe((mailInfo: MailPreviewInfo) => {
      if (!mailInfo) return;

      this._mail.sendMail(mailInfo).subscribe(res => {
        if (res?.success) {
          this.showToastMessage(ToastMessageType.SUCCESS, res.result);
        }
      });
    });
  }

  showEdit(mailData: Mail) {
    const dialogRef = this.dialogService.open(EditMailDialogComponent, {
      header: `Edit ${mailData.name} Template`,
      width: '90%',
      height: "80vh",
      contentStyle: { 'background-color': 'rgba(242,245,245)', overflow: 'auto' },
      baseZIndex: 10000,
      data: { templateId: mailData.id }
    });
    dialogRef.onClose.subscribe((res: Mail) => res && (this.refresh()));
  }

  ngOnDestroy() {
    super.ngOnDestroy();
    if (this.dialogRef) this.dialogRef.close()
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Admin", routerLink: DefaultRoute.Admin, styleClass: 'menu-item-click' }, { label: "Mails" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}
