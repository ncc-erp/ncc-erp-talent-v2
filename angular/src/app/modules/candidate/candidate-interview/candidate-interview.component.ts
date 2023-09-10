import { Component, Injector, OnInit } from "@angular/core";
import { checkNumber } from "@app/core/helpers/utils.helper";
import { CandidateInfo } from '@app/core/models/candidate/candidate.model';
import { RequisitionInfo } from '@app/core/models/requisition/requisition.model';
import { CandidateInterview, CandidateInterviewPayloadList } from './../../../core/models/candidate/candidate-interview.model';

import { CandidateInterviewService } from '@app/core/services/candidate/candidate-interview.service';
import { UtilitiesService } from "@app/core/services/utilities.service";
import { DateFormat, FILTER_TIME } from "@shared/AppConsts";
import {
  API_RESPONSE_STATUS, COMPARISION_OPERATOR, CreationTimeEnum, REQUEST_CV_STATUS, SortType
} from "@shared/AppEnums";
import { TalentDateTime } from '@shared/components/date-selector/date-selector.component';
import {
  Filter,
  PagedListingComponentBase,
  PagedRequestDto
} from "@shared/paged-listing-component-base";
import { DialogService } from 'primeng/dynamicdialog';

@Component({
  selector: "talent-candidate-interview",
  templateUrl: "./candidate-interview.component.html",
  styleUrls: ["./candidate-interview.component.scss"],
})
export class CandidateInterviewComponent extends PagedListingComponentBase<CandidateInterview> implements OnInit {
  public readonly SORT_TYPE = SortType;
  public readonly DATE_FORMAT = DateFormat;
  public readonly FILTER_TIME = FILTER_TIME;
  public readonly REQUEST_CV_STATUS = REQUEST_CV_STATUS;

  filterInterviewerConfig = {
    labelName: "Interviewer",
    catalogList: this._utilities.catAllInterviewer,
    optionLabel: "labelName",
    optionValue: "id",
    username: "username"
  };

  searchDetail = {
    userType: null,
    requestCVStatus: REQUEST_CV_STATUS.ScheduledInterview,
  }

  candidateInterviews: CandidateInterview[] = [];
  searchWithInterviewerIds: number[];
  searchWithinterviewTime: TalentDateTime = null

  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    public _dialog: DialogService,
    private _candidateInterview: CandidateInterviewService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload: any = this.getPayLoad(request);
    this.subs.add(
      this._candidateInterview.getAllPagging(payload).subscribe((rs) => {
        this.candidateInterviews = [];
        if (rs.success) {
          this.candidateInterviews = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(entity: CandidateInterview): void {
    const deleteRequest = this._candidateInterview.delete(entity.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, `#${entity.id}`).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  onInterViewTimeChange(talentDateTime: TalentDateTime) {
    this.searchWithinterviewTime = talentDateTime;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  onGetDataChange() {
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  onInterviewerChange(event) {
    this.searchWithInterviewerIds = event;
    this.onGetDataChange();
  }

  private getPayLoad(request: PagedRequestDto): CandidateInterviewPayloadList {
    const payload: any = { ...request };
    this.searchWithInterviewerIds?.length && (payload.interviewerIds = this.searchWithInterviewerIds);

    const filterItems: Filter[] = [];

    let filterObj: Filter = null;
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

    const interviewTime: TalentDateTime = this.searchWithinterviewTime;
    if (interviewTime && interviewTime.dateType !== CreationTimeEnum.ALL) {
      const fromfilterObj: Filter = {
        propertyName: "interviewTime",
        value: interviewTime?.fromDate.format(DateFormat.YYYY_MM_DD),
        comparision: COMPARISION_OPERATOR.GreaterThanOrEqual,
      };

      const tofilterObj: Filter = {
        propertyName: "interviewTime",
        value: interviewTime?.toDate.format(DateFormat.YYYY_MM_DD),
        comparision: COMPARISION_OPERATOR.LessThanOrEqual,
      };

      filterItems.push(fromfilterObj);
      filterItems.push(tofilterObj);
    }

    payload.filterItems = filterItems;
    return payload;
  }

  getCandidateInfo(entity: CandidateInterview): CandidateInfo {
    const candidateInfo = {
      ...entity,
      id: entity.cvId,
    };
    return candidateInfo;
  }

  getRequisitionInfo(entity: CandidateInterview): RequisitionInfo {
    const requisitionInfo = {
      ...entity,
      id: entity.requestId,
      branchName: entity.requestBranchName,
      subPositionName: entity.requestSubPositionName,
    };
    return requisitionInfo;
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "My Interview" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
}
