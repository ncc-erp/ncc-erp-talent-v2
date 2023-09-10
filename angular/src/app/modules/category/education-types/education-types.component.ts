import { DefaultRoute } from './../../../../shared/AppEnums';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { Component, Injector, OnDestroy, OnInit } from "@angular/core";
import { EducationType, EducationTypeConfigDiaLog } from '@app/core/models/categories/education-type.model';
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
import { EducationTypeService } from "../../../core/services/categories/education-type.service";
import { EducationTypesDialogComponent } from './education-types-dialog/education-types-dialog.component';
import { MESSAGE } from '@shared/AppConsts';

@Component({
  selector: "talent-education-types",
  templateUrl: "./education-types.component.html",
  styleUrls: ["./education-types.component.scss"],
})
export class EducationTypesComponent extends PagedListingComponentBase<EducationType> implements OnInit, OnDestroy {

  public educationTypes: EducationType[] = [];
  private dialogRef: DynamicDialogRef;

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _educationType: EducationTypeService,
    private _utilities: UtilitiesService
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

  openDialog(obj: EducationType, dialogAction: ActionEnum) {
    const dialogConfig: EducationTypeConfigDiaLog = { educationType: obj, action: dialogAction };
    const dialogRef = this.dialogService.open(EducationTypesDialogComponent, {
      header: `${dialogConfig.action} Education Type`,
      width: "40%",
      contentStyle: { "max-height": "500px", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<EducationType>) => {
      if (dialogConfig.action === ActionEnum.UPDATE) {
        const index = this.educationTypes.findIndex((x) => x.id == res.result.id);
        this.educationTypes[index] = res.result;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.name);
        return;
      }
      this.refresh();
    });
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    this.subs.add(
      this._educationType.getAllPagging(request).subscribe((rs) => {
        this.educationTypes = [];
        if (rs.success) {
          this.educationTypes = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(educationType: EducationType): void {
    const deleteRequest = this._educationType.delete(educationType.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, educationType.name).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this._utilities.loadCatalogForCategories();
          this.refresh();
        }
      })
    );
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [
        { label: "Categories", routerLink: DefaultRoute.Category, styleClass: 'menu-item-click' },
        { label: "Education Types" }
      ],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}
