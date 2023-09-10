import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { API_RESPONSE_STATUS, ActionEnum, COMPARISION_OPERATOR, DefaultRoute, ToastMessageType } from '@shared/AppEnums';
import { ApiResponse, Filter, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { DialogScoreSettingComponent } from './dialog-score-setting/dialog-score-setting.component';
import { ScoreSetting, ScoreRangeWithSetting, ScoreSettingDialogData } from '@app/core/models/categories/score-range-setting.model';
import { ScoreSettingService } from '@app/core/services/categories/score-setting.service';
import { MESSAGE } from '@shared/AppConsts';
import { checkNumber } from '@app/core/helpers/utils.helper';

@Component({
  selector: 'talent-score-setting',
  templateUrl: './score-setting.component.html',
  styleUrls: ['./score-setting.component.scss'],
})
export class ScoreSettingComponent extends PagedListingComponentBase<ScoreRangeWithSetting> implements OnInit, OnDestroy {

  searchWithUserType: number = null;
  searchWithSubPosition: number = null;
  searchWithFrom: boolean = null;
  scoreSetting: ScoreSetting[] = [];
  editting: boolean = true;
  ref: DynamicDialogRef;
  isRowExpand: boolean = false;
  expandedRows: number[] = [];

  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    private _scoreSetting: ScoreSettingService,
    private dialogService: DialogService,
  ) {
    super(injector);

  }

  openDialog(dialogAction: ActionEnum, scoreSetting?: ScoreSetting, scoreRange?: ScoreRangeWithSetting) {
    const diaLogScoreData: ScoreSettingDialogData = { dialogAction: dialogAction, scoreSetting: scoreSetting, scoreRange: scoreRange };
    const dialogRef = this.dialogService.open(DialogScoreSettingComponent, {
      header: `${dialogAction} Score Setting`,
      width: '900px',
      contentStyle: { overflow: 'auto' },
      data: diaLogScoreData,
    });

    dialogRef.onClose.subscribe((res?: ApiResponse<ScoreSetting[]>) => {
      if (res?.success) {
        this.getDataPage(this.GET_FIRST_PAGE);
        this.checkNotifacation(diaLogScoreData);
      }
    });
  }

  checkNotifacation(diaLogScoreData: ScoreSettingDialogData) {
    if (diaLogScoreData.dialogAction === ActionEnum.UPDATE) {
      this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);
    }
    else if (diaLogScoreData.dialogAction === ActionEnum.CREATE) {
      this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CREATE_SUCCESS);
    }
  }

  onPositionSelect(subPositionId: number) {
    this.searchWithSubPosition = subPositionId;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  getRowspanCapSetting(item: ScoreSetting) {
    if (!item.scoreRanges || item.scoreRanges.length === 0) {
      return 1;
    }
    return item.scoreRanges.length;
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
  }

  protected delete(scoreRange: ScoreRangeWithSetting): void {
    const deleteRequest = this._scoreSetting.delete(scoreRange.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, `#${scoreRange.levelInfo.standardName}`).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  onGetDataChange() {
    this.resetRowExpand();
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  private resetRowExpand() {
    this.isRowExpand = false;
    this.expandedRows = [];
  }

  private getPayLoad(request: PagedRequestDto) {
    const payload: any = { ...request }
    const filterItems: Filter[] = [];

    if (checkNumber(this.searchWithUserType)) {
      const filterObj: Filter = { propertyName: 'userType', value: this.searchWithUserType, comparision: COMPARISION_OPERATOR.Equal }
      filterItems.push(filterObj);
    }

    if (checkNumber(this.searchWithSubPosition)) {
      const filterObj: Filter = { propertyName: 'subPositionId', value: this.searchWithSubPosition, comparision: COMPARISION_OPERATOR.Equal }
      filterItems.push(filterObj);
    }

    payload.filterItems = filterItems;
    return payload;
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload = this.getPayLoad(request);
    this.subs.add(
      this._scoreSetting.getAllPagging(payload).subscribe((rs) => {
        this.scoreSetting = [];
        if (rs.success) {
          this.scoreSetting = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Categories", routerLink: DefaultRoute.Category, styleClass: 'menu-item-click' }, { label: "Score Setting" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}