import { Component, Injector, OnInit } from '@angular/core';
import { PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { DialogService, DynamicDialogRef } from "primeng/dynamicdialog";
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { API_RESPONSE_STATUS, DefaultRoute, ToastMessageType } from '@shared/AppEnums';
import { BackgroundWorkerDto } from 'app/core/models/background-worker/background-worker.model'
import { BackgroundWorkerService } from 'app/core/services/background-worker/background-worker.service';
import { MenuItem } from '@node_modules/primeng/api/menuitem';
import { copyObject } from '@app/core/helpers/utils.helper';
import { MESSAGE } from '@shared/AppConsts';

@Component({
  selector: 'talent-background-worker',
  templateUrl: './background-worker.component.html',
  styleUrls: ['./background-worker.component.scss']
})
export class BackgroundWorkerComponent extends PagedListingComponentBase<BackgroundWorkerDto> implements OnInit {
  backgroundWorkers: BackgroundWorkerDto[] = [];
  clonedbackgroundWorkers: { [s: string]: BackgroundWorkerDto } = {};
  isActive: boolean | undefined = undefined;
  dialogRef: DynamicDialogRef;
  editingRowKey: { [s: string]: boolean } = {};

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _backgroundWorkerService: BackgroundWorkerService,
    private _modalService: BsModalService
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
    request.sort = 'Id';
    request.sortDirection = 1;

    this.subs.add(
      this._backgroundWorkerService.getAllPagging(request).subscribe((rs) => {
        this.backgroundWorkers = [];
        if (rs.success) {
          this.backgroundWorkers = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  onSave(worker: BackgroundWorkerDto) {
    this.subs.add(
      this._backgroundWorkerService.update(worker).subscribe((res) => {
        this.isLoading = res.loading;
        if (res.error) {
          this.onCancel(worker);
          return;
        }

        if (!res.loading && res.result && res.success) {
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.Name);
          this.refresh();
        }
      })
    );
  }

  public getListItem(worker: BackgroundWorkerDto): MenuItem[] {
    return [{
      label: 'Action',
      items: [{
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => {
          this.onEdit(worker);
          this.editingRowKey[worker.id] = true;
        },
        visible: this.permission.isGranted(this.PS.Pages_BackgroundWorkers_Update),
      }]
    }]
  }

  onEdit(entity: BackgroundWorkerDto) {
      this.clonedbackgroundWorkers[entity.id] = copyObject(entity);
  }
  
  onCancel(entity: BackgroundWorkerDto) {
    const i = this.backgroundWorkers.findIndex((item) => item.id === entity.id);
    this.backgroundWorkers[i] = this.clonedbackgroundWorkers[entity.id];
  }

  protected delete(entity: BackgroundWorkerDto): void {
    return;
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Admin", routerLink: DefaultRoute.Admin, styleClass: 'menu-item-click' }, { label: "Background Worker" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}
