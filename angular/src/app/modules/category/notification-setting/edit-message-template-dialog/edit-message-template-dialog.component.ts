import { AppComponentBase } from '@shared/app-component-base';
import { Component, Injector, OnInit, Optional } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageTemplateDto } from '@app/core/models/categories/notification-setting.model';
import { NotificationSettingService } from '@app/core/services/categories/notification-setting.service';

@Component({
  selector: 'talent-edit-message-template-dialog',
  templateUrl: './edit-message-template-dialog.component.html',
  styleUrls: ['./edit-message-template-dialog.component.scss']
})
export class EditMessageTemplateDialogComponent extends AppComponentBase implements OnInit {
  template: MessageTemplateDto = new MessageTemplateDto();
  notificationId: number;
  isSaving: boolean = false;
  items: number[] = [0];

  constructor(
    injector: Injector,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig,
    private _notificationSettingService: NotificationSettingService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    Object.assign(this, this.config.data);
    this.getTemplateById()
  }

  getTemplateById() {
    this._notificationSettingService.getMessageTemplateById(this.notificationId).subscribe(
      (res) => {
        if (res?.success) {
          this.template = res.result;
        }
    })
  }

  addProperty(property: string) { 
    const parentElement = document.getElementById("numberitiont");
    if (parentElement) {
      const selectedText = document.getSelection();
      const selectedRange = selectedText.getRangeAt(0);
      if (parentElement.contains(selectedRange.startContainer)) {
          const selectedTextContent = selectedRange.toString();
          if (/^\s*$/.test(selectedTextContent)) {
            selectedRange.deleteContents();
          }
          selectedRange.deleteContents();
          const propertyNode = document.createTextNode(`{{${property}}}`);
          selectedRange.insertNode(propertyNode);
          selectedRange.setStartAfter(propertyNode);
          selectedRange.setEndAfter(propertyNode);
          selectedText.removeAllRanges();  
          selectedText.addRange(selectedRange);
        }
      }
    }
  
  save() {
    if (this.ref) this.ref.close(this.template);
  }
}

