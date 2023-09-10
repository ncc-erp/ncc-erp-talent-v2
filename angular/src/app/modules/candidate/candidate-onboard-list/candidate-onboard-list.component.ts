import { RequisitionInfo } from './../../../core/models/requisition/requisition.model';
import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { checkNumber, copyObject } from '@app/core/helpers/utils.helper';
import { CandidateInfo } from '@app/core/models/candidate/candidate.model';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { DateFormat, MESSAGE,FILTER_TIME } from '@shared/AppConsts';
import { COMPARISION_OPERATOR, CreationTimeEnum, REQUEST_CV_STATUS, SortType, ToastMessageType } from '@shared/AppEnums';
import { Filter, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import * as moment from 'moment';
import { CandidateOnboard, CandidateOnboardPayload } from './../../../core/models/candidate/candidate-onboard.model';
import { CandidateOnboardService } from './../../../core/services/candidate/candidate-onboard.service';
import { TalentDateTime } from '@shared/components/date-selector/date-selector.component';
import { Table } from 'primeng/table';
import { MenuItem } from 'primeng/api';
@Component({
  selector: 'talent-candidate-onboard-list',
  templateUrl: './candidate-onboard-list.component.html',
  styleUrls: ['./candidate-onboard-list.component.scss']
})
export class CandidateOnboardListComponent extends PagedListingComponentBase<CandidateOnboard> implements OnInit {
  @ViewChild('tableOnboard') tableOnboard: Table;

  public readonly SORT_TYPE = SortType;
  public readonly DATE_FORMAT = DateFormat;
  public readonly FILTER_TIME = FILTER_TIME;
  readonly IN_PROGRESS = 'In Progress';
  editingRowKey: { [s: string]: boolean } = {};

  candOnboards: CandidateOnboard[] = []
  clonedCandOnboard: { [s: string]: CandidateOnboard; } = {};

  searchDetail = {
    requestCVStatus: REQUEST_CV_STATUS.AcceptedOffer,
    userType: null,
    branchId: null,
    subPositionId: null,
    requestStatus: this._utilities.catReqStatus.find(item => item.name === this.IN_PROGRESS).id ?? null
  }

  searchWithOnboardDate: TalentDateTime;

  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    private _candidateOnboard: CandidateOnboardService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadCrumbConfig();
   }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload = this.getPayLoad(request);
    this.subs.add(
      this._candidateOnboard.getAllPagging(payload).subscribe((rs) => {
        this.candOnboards = [];
        if (rs.success) {
          this.candOnboards = rs.result.items;
          this.showPaging(rs.result, pageNumber);

        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(entity: CandidateOnboard): void { return; }

  onPositionSelect(subPositionId: number) {
    this.searchDetail.subPositionId = subPositionId;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  onSave(entity: CandidateOnboard) {
    const payload: CandidateOnboardPayload = {
      id: entity.id,
      status: entity.requestCVStatus,
      onboardDate: entity.onboardDate && moment(entity.onboardDate).format(DateFormat.YYYY_MM_DD),
      nccEmail: entity.nccEmail,
      hrNote: entity.hrNote
    };

    this.subs.add(
      this._candidateOnboard.update(payload).subscribe(res => {
        this.isLoading = res.loading;
        if (res.error) {
          this.tableOnboard.initRowEdit(entity);
          return;
        }

        if (!res.loading && res.success && res.result) {
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, res.result.fullName);
          this.refresh();
        }
      })
    );
  }

  onEdit(entity: CandidateOnboard) {
    entity.onboardDate = (entity.onboardDate && new Date(entity.onboardDate)) || null;
    this.clonedCandOnboard[entity.id] = copyObject(entity);
  }

  onEditCancel(entity: CandidateOnboard) {
    const idx: number = this.candOnboards.findIndex(item => item.id === entity.id);
    this.candOnboards[idx] = this.clonedCandOnboard[entity.id];
  }

  getCandidateInfo(entity: CandidateOnboard): CandidateInfo {
    return {
      ...entity,
      id: entity.cvId
    }
  }

  getRequisitionInfo(entity: CandidateOnboard): RequisitionInfo {
    return {
      ...entity,
      id: entity.requestId,
      requestStatusName: entity.requestStatusName,
      branchName: entity.requestBranchName,
      subPositionName: entity.subPositionNameRequest,
    }
  }

  onTalentDateChange(talentDateTime: TalentDateTime) {
    this.searchWithOnboardDate = talentDateTime;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  private getPayLoad(request: PagedRequestDto) {
    const payload: any = { ...request }
    const filterItems: Filter[] = [];

    let filterObj: Filter = null;

    if (this.searchWithOnboardDate && this.searchWithOnboardDate.dateType !== CreationTimeEnum.ALL) {
      payload.fromDate = this.searchWithOnboardDate?.fromDate?.format(DateFormat.YYYY_MM_DD);
      payload.toDate = this.searchWithOnboardDate?.toDate?.format(DateFormat.YYYY_MM_DD);
    }

    for (const property in this.searchDetail) {
      if (checkNumber(this.searchDetail[property])) {
        filterObj = {
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

  private getBreadCrumbConfig() {
    return {
      menuItem: [{ label: "Candidate Onboard" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

  public updateToHRM(requestCvId:number){
    this.subs.add(
    this._candidateOnboard.UpdateHrmTempEmployee(requestCvId)
    .subscribe(rs => {
      if (!rs.loading && rs.result && rs.success) {
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, "Updated employee to HRM");
        this.refresh();
      }
    })
    )
  }

  public getListItem(candidateInfo:CandidateOnboard): MenuItem[] {
    return [{
      label: 'Action',
      items: [{
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => {
          this.onEdit(candidateInfo);
          this.editingRowKey[candidateInfo.id] = true;
        },
      },
      {
        label: 'Update to HRM',
        icon: 'pi pi-user-plus',
        command: () => {
          this.updateToHRM(candidateInfo.id);
        },
        visible: candidateInfo.requestCVStatus == REQUEST_CV_STATUS.AcceptedOffer
        || candidateInfo.requestCVStatus == REQUEST_CV_STATUS.RejectedOffer
      }]
    }]
  }


}
