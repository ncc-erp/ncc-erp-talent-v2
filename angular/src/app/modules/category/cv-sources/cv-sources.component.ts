import { MESSAGE } from '@shared/AppConsts';
import { DefaultRoute } from './../../../../shared/AppEnums';
import { Component, Injector, OnDestroy, OnInit } from "@angular/core";
import { CVSource, CVSourceConfigDiaLog } from '@app/core/models/categories/cv-source.model';
import {
  ActionEnum,
  API_RESPONSE_STATUS, ToastMessageType
} from "@shared/AppEnums";
import {
  ApiResponse,
  PagedListingComponentBase,
  PagedRequestDto
} from "@shared/paged-listing-component-base";
import { DialogService, DynamicDialogRef } from "primeng/dynamicdialog";
import { CvSourceService } from './../../../core/services/categories/cv-source.service';
import { CvSourceDialogComponent } from './cv-source-dialog/cv-source-dialog.component';

@Component({
  selector: 'talent-cv-sources',
  templateUrl: './cv-sources.component.html',
  styleUrls: ['./cv-sources.component.scss']
})
export class CvSourcesComponent extends PagedListingComponentBase<CVSource> implements OnInit, OnDestroy {

  public cvSources: CVSource[] = [];
  private dialogRef: DynamicDialogRef;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _cvSource: CvSourceService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
  }

  ngOnDestroy() {
    super.ngOnDestroy();
    if (this.dialogRef) this.dialogRef.close()
  }

  openDialog(obj: CVSource, dialogAction: ActionEnum) {
    const dialogConfig: CVSourceConfigDiaLog = { cvSource: obj, action: dialogAction };
    const dialogRef = this.dialogService.open(CvSourceDialogComponent, {
      header: `${dialogConfig.action} CV Source`,
      width: "40%",
      contentStyle: { "max-height": "500px", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<CVSource>) => {
      if (dialogConfig.action === ActionEnum.UPDATE && res) {
        const index = this.cvSources.findIndex((x) => x.id == res.result.id);
        this.cvSources[index] = res.result;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.name);
        return;
      }
      this.refresh();
    });
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subs.add(
      this._cvSource.getAllPagging(request).subscribe((rs) => {
        this.cvSources = [];
        if (rs.success) {
          this.cvSources = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(cvsource: CVSource): void {
    const deleteRequest = this._cvSource.delete(cvsource.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, cvsource.name).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Categories", routerLink: DefaultRoute.Category, styleClass: 'menu-item-click' }, { label: "CV Sources" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

}
