import { Component, Injector, Input, OnInit } from '@angular/core';
import { DateFormat, MESSAGE } from '@shared/AppConsts';
import { ActionEnum, API_RESPONSE_STATUS, DefaultRoute, ToastMessageType } from '@shared/AppEnums';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageTemplateDto, NotificationSetting } from '@app/core/models/categories/notification-setting.model';
import { MenuItem } from '@node_modules/primeng/api/menuitem';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { NotificationSettingService } from '@app/core/services/categories/notification-setting.service';
import { copyObject } from '@app/core/helpers/utils.helper';
import { EditMessageTemplateDialogComponent } from '../notification-setting/edit-message-template-dialog/edit-message-template-dialog.component';
import { PreviewMessageTemplateDialogComponent } from './preview-message-template-dialog/preview-message-template-dialog.component';

@Component({
  selector: 'talent-notification-setting',
  templateUrl: './notification-setting.component.html',
  styleUrls: ['./notification-setting.component.scss']
})
export class NotificationSettingComponent extends PagedListingComponentBase<NotificationSetting> implements OnInit {
  notifications: NotificationSetting[] = [];
  clonednotifications: { [s: string]: NotificationSetting } = {};
  editingRowKey: { [s: string]: boolean } = {};
  dialogRef: DynamicDialogRef;
  isActive: boolean | undefined = undefined;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _notificationSettingService: NotificationSettingService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
  }

  protected list(
    request: PagedRequestDto, 
    pageNumber: number, 
    finishedCallback: Function
  ): void {
    request.sort = 'Type';
    request.sortDirection = 0;

    this.subs.add(
      this._notificationSettingService.getAllPagging(request).subscribe(rs => {
        this.notifications = [];
        this.isLoading = rs.loading;
        if (rs.success) {
          this.notifications = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  onSave(notification: NotificationSetting) {
    this.subs.add(
      this._notificationSettingService.update(notification).subscribe((res) => {
        this.isLoading = res.loading;
        if (res.error) {
          this.onCancel(notification);
          return;
        }

        if (!res.loading && res.result && res.success) {
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.Name);
          this.refresh();
        }
      })
    );
  }

  protected delete(entity: NotificationSetting): void {

  }

  public getListItem(notification: NotificationSetting): MenuItem[] {
    return [{
      label: 'Action',
      items: [{
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => {
          this.onEdit(notification);
          this.editingRowKey[notification.id] = true;
        },
        visible: this.permission.isGranted(this.PS.Pages_NotificationSettings_Update),
      },
      {
        label: 'Edit Template',
        icon: 'fas fa-edit',
        command: () => {
          this.showEdit(notification);
        },
        visible: this.permission.isGranted(this.PS.Pages_NotificationSettings_Update),
      },
      {
        label: 'Preview Template',
        icon: 'fas fa-eye',
        command: () => {
          this.showPreview(notification);
        },
        visible: this.permission.isGranted(this.PS.Pages_NotificationSettings_Preview),
      }]
    }]
  }

  onEdit(entity: NotificationSetting) {
      this.clonednotifications[entity.id] = copyObject(entity);
  }
    
  onCancel(entity: NotificationSetting) {
    const i = this.notifications.findIndex((item) => item.id === entity.id);
    this.notifications[i] = this.clonednotifications[entity.id];
  }

  showEdit(entity: NotificationSetting){
    const dialogRef = this.dialogService.open(EditMessageTemplateDialogComponent, {
      header: `Edit ${entity.typeName} Template`,
      width: '75%',
      height: "70vh",
      contentStyle: { 'background-color': 'rgba(242,245,245)', overflow: 'auto' },
      baseZIndex: 10000,
      data: { notificationId: entity.id }
    });
    dialogRef.onClose.subscribe((res: MessageTemplateDto) => 
    {
      entity.bodyMessage = res.bodyMessage;
      this.onSave(entity);
    });
  }

  showPreview(entity: NotificationSetting){
    const dialogRef = this.dialogService.open(PreviewMessageTemplateDialogComponent, {
      header: `Preview ${entity.typeName} Template`,
      width: '55%',
      contentStyle: { 'max-height': '600px', 'background-color': 'rgba(242,245,245)', overflow: 'auto' },
      baseZIndex: 10000,
      data: { notificationId: entity.id }
    });
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [
        {
          label: "Categories",
          routerLink: DefaultRoute.Category,
          styleClass: "menu-item-click",
        },
        { label: "Notification Setting" },
      ],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
  
}
