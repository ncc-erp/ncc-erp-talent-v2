import { ToastMessageType } from '@shared/AppEnums';
import { MailPreviewInfo } from './../../../../core/models/mail/mail.model';
import { AppComponentBase } from '@shared/app-component-base';
import { Component, Injector, OnInit, Optional } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MailService } from '@app/core/services/apis/mail.service';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { MESSAGE } from '@shared/AppConsts';

@Component({
  selector: 'talent-edit-mail-dialog',
  templateUrl: './edit-mail-dialog.component.html',
  styleUrls: ['./edit-mail-dialog.component.scss']
})
export class EditMailDialogComponent extends AppComponentBase implements OnInit {
  mailInfo: MailPreviewInfo;
  templateId: number;
  isSaving: boolean = false;
  items: number[] = [0];

  constructor(
    injector: Injector,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private _mail: MailService,

  ) {
    super(injector);
  }

  ngOnInit(): void {
    Object.assign(this, this.config.data);
    if (this.templateId) this.getTemplateById();
  }

  getTemplateById() {
    this.subs.add(
      this._mail.getById(this.templateId).subscribe((res: ApiResponse<MailPreviewInfo>) => {
        this.mailInfo = new MailPreviewInfo();
        if (res?.success) this.mailInfo = res.result;
      })
    )
  }

  addProperty(property: string) {  
    const selectedText  = window.getSelection();
    const selectedRange  = selectedText .getRangeAt(0);

    const startContainer  = selectedRange .startContainer;
    const selectedTextContent  = startContainer .textContent;
  
    const startIndex = selectedTextContent .lastIndexOf('{{', selectedRange .startOffset);
    const endIndex = selectedTextContent .indexOf('}}', selectedRange .startOffset);

    if (startIndex >= 0 && endIndex >= 0) {
      const newText = selectedTextContent .substring(0, startIndex) + `{{${property}}}` + selectedTextContent .substring(endIndex + 2);
      startContainer .textContent = newText;
    } else {
      const propertyNode = document.createTextNode(`{{${property}}}`);  
      selectedRange .insertNode(propertyNode);
      selectedRange .setStartAfter(propertyNode);
      selectedRange .setEndAfter(propertyNode);  
      selectedText .removeAllRanges();
      selectedText .addRange(selectedRange );
    }
  }

  save() {
    if(this.templateId) {
      this.handleSaveTemplate();
      return;
    }

    if (this.ref) this.ref.close(this.mailInfo);
  }

  private handleSaveTemplate() {
    this._mail.update(this.mailInfo).subscribe((res: ApiResponse<string>) => {
      this.isSaving = res.loading;
      if (res?.success) {
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);
        this.ref.close(res.result);
      }
    })
  }
}
