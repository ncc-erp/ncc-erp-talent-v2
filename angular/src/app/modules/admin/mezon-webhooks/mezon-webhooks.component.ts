import { Component, Injector, OnInit } from '@angular/core';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { MezonWebhookDto } from 'app/core/models/mezon-webhook/mezon-webhook.model';
import { MezonWebhookService } from 'app/core/services/mezon-webhook/mezon-webhook.service';
import { DialogService, DynamicDialogRef } from "primeng/dynamicdialog";
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { EditMezonWebhookDialogComponent } from './edit-mezon-webhook-dialog/edit-mezon-webhook-dialog.component';
import { API_RESPONSE_STATUS, DefaultRoute } from '@shared/AppEnums';
import { CreateMezonWebhookDialogComponent } from './create-mezon-webhook-dialog/create-mezon-webhook-dialog.component';

@Component({
  templateUrl: './mezon-webhooks.component.html',
  styleUrls: ['./mezon-webhooks.component.scss'],
  animations: [appModuleAnimation()]
})
export class MezonWebhooksComponent extends PagedListingComponentBase<MezonWebhookDto> implements OnInit {
  mezonWebhooks: MezonWebhookDto[] = [];
  isActive: boolean | undefined = undefined;
  dialogRef: DynamicDialogRef;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _mezonWebhookService: MezonWebhookService,
    private _modalService: BsModalService
  ) {
    super(injector);
    this.breadcrumbConfig = this.getBreadcrumbConfig();
  }

  ngOnInit(): void {
  }

  protected list(
    request: PagedRequestDto, 
    pageNumber: number, 
    finishedCallback: Function
  ): void {
    request.sort = 'Id';
    request.sortDirection = 1;

    this.subs.add(
      this._mezonWebhookService.getAllPagging(request).subscribe((rs) => {
        this.mezonWebhooks = [];
        if (rs.success) {
          this.mezonWebhooks = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  editWebhook(webhook: MezonWebhookDto): void {
    let editWebhookDialog: BsModalRef;
    editWebhookDialog = this._modalService.show(
      EditMezonWebhookDialogComponent,
      {
        class: 'modal-lg',
        initialState: {
          id: webhook.id,
        },
      }
    );
    editWebhookDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  createWebhook(): void{
    let createWebhookDialog: BsModalRef;
    createWebhookDialog = this._modalService.show(
      CreateMezonWebhookDialogComponent,
      {
        class: 'modal-lg'
      }
    );
    createWebhookDialog.content.onSave.subscribe(() => {
      this.refresh();
    });
  }

  protected delete(entity: MezonWebhookDto): void {
    const deleteRequest = this._mezonWebhookService.delete(entity.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, entity.name).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Admin", routerLink: DefaultRoute.Admin, styleClass: 'menu-item-click' }, { label: "Mezon Webhooks" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}
