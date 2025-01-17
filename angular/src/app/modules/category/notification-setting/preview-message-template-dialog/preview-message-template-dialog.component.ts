import { AppComponentBase } from '@shared/app-component-base';
import { Component, Injector, OnInit, Optional } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageTemplateDto, NotificationSetting } from '@app/core/models/categories/notification-setting.model';
import { NotificationSettingService } from '@app/core/services/categories/notification-setting.service';


@Component({
  selector: 'talent-preview-message-template-dialog',
  templateUrl: './preview-message-template-dialog.component.html',
  styleUrls: ['./preview-message-template-dialog.component.scss']
})
export class PreviewMessageTemplateDialogComponent extends AppComponentBase implements OnInit {
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
    this._notificationSettingService.getFakeData(this.notificationId).subscribe(
      (res) => {
        if (res?.success) this.template = res.result;
    })
  }
  
}