import { MESSAGE } from '@shared/AppConsts';
import { Component, Injector, OnDestroy, OnInit,ElementRef } from "@angular/core";
import { SubPositionService } from './../../../core/services/categories/sub-position.service';
import {
  ActionEnum,
  API_RESPONSE_STATUS, COMPARISION_OPERATOR, ToastMessageType
} from "@shared/AppEnums";
import {
  ApiResponse,
  Filter,
  PagedListingComponentBase,
  PagedRequestDto
} from "@shared/paged-listing-component-base";
import { DialogService, DynamicDialogRef } from "primeng/dynamicdialog";
import { DefaultRoute } from './../../../../shared/AppEnums';
import { SubPosition, SubPositionConfigDiaLog } from './../../../core/models/categories/sub-position.model';
import { SubPositionDialogComponent } from './sub-position-dialog/sub-position-dialog.component';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { checkNumber } from '@app/core/helpers/utils.helper';


@Component({
  selector: 'talent-sub-position',
  templateUrl: './sub-position.component.html',
  styleUrls: ['./sub-position.component.scss']
})
export class SubPositionComponent extends PagedListingComponentBase<SubPosition> implements OnInit, OnDestroy {
  public subPositions: SubPosition[] = [];
  private dialogRef: DynamicDialogRef;
  public searchDetail = {
      PositionId: null,
  };
  constructor(
    injector: Injector,
    public dialogService: DialogService,
    private _subPosition: SubPositionService,
    public _utilities: UtilitiesService
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

  openDialog(obj: SubPosition, dialogAction: ActionEnum) {
    const dialogConfig: SubPositionConfigDiaLog = { subPosition: obj, action: dialogAction };
    const dialogRef = this.dialogService.open(SubPositionDialogComponent, {
      header: `${dialogConfig.action} Sub Position`,
      width: "40%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      baseZIndex: 10000,
      data: dialogConfig,
    });

    dialogRef.onClose.subscribe((res: ApiResponse<SubPosition>) => {
      if (dialogConfig.action === ActionEnum.UPDATE) {
        const index = this.subPositions.findIndex((x) => x.id == res.result.id);
        this.subPositions[index] = res.result;
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.name);
        return;
      }
      this.refresh();
    });
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload = this.getPayLoad(request)
    this.subs.add(
      this._subPosition.getAllPagging(payload).subscribe((rs) => {
        this.subPositions = [];
        if (rs.success) {
          this.subPositions = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(position: SubPosition): void {
    const deleteRequest = this._subPosition.delete(position.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, position.name).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Categories", routerLink: DefaultRoute.Category, styleClass: 'menu-item-click' }, { label: "Sub Positions" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
  onFilter(event) {
    this.searchDetail.PositionId = event.value;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  private getPayLoad(request: PagedRequestDto) {
    const payload: any = { ...request }
    const filterItems: Filter[] = [];
    for (const property in this.searchDetail) {
      if (checkNumber(this.searchDetail[property])) {
        const filterObj = {
          propertyName: property,
          value: this.searchDetail[property],
          comparision: COMPARISION_OPERATOR.Equal,
        }
        filterItems.push(filterObj);
      }
    }

    payload.filterItems = filterItems;
    return payload;
  }
}
