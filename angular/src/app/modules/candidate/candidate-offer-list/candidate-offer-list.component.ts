import { Component, Injector, OnInit } from "@angular/core";
import { UtilitiesService } from "@app/core/services/utilities.service";

import { checkNumber, copyObject } from "@app/core/helpers/utils.helper";
import {
  CandidateOffer,
  CandidateOfferPayload
} from "@app/core/models/candidate/candidate-offer.model";
import { CandidateInfo } from "@app/core/models/candidate/candidate.model";
import { RequisitionInfo } from '@app/core/models/requisition/requisition.model';
import { CandidateOfferService } from "@app/core/services/candidate/candidate-offer.service";
import { DateFormat, MESSAGE } from "@shared/AppConsts";
import {
  COMPARISION_OPERATOR,
  REQUEST_CV_STATUS,
  SortType,
  ToastMessageType,
  UserType
} from "@shared/AppEnums";
import {
  Filter,
  PagedListingComponentBase,
  PagedRequestDto
} from "@shared/paged-listing-component-base";
import * as moment from "moment";
import { MenuItem } from "primeng/api";

@Component({
  selector: "talent-candidate-offer-list",
  templateUrl: "./candidate-offer-list.component.html",
  styleUrls: ["./candidate-offer-list.component.scss"],
})
export class CandidateOfferListComponent extends PagedListingComponentBase<CandidateOffer> implements OnInit {

  readonly SORT_TYPE = SortType;
  readonly DATE_FORMAT = DateFormat;
  readonly USER_TYPE = UserType;

  candidateOffers: CandidateOffer[] = [];
  clonedCandidateOffer: { [s: string]: CandidateOffer } = {};
  editingRowKey: { [s: string]: boolean } = {};
  searchDetail = {
    requestCVStatus: REQUEST_CV_STATUS.PassedInterview,
    finalLevel: null,
    userType: null,
    branchId: null,
    subPositionId: null,
  };

  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    private _candidateOffer: CandidateOfferService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadCrumbConfig();
  }

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    const payload = this.getPayload(request);
    this.subs.add(
      this._candidateOffer.getAllPagging(payload).subscribe((rs) => {
        this.candidateOffers = [];
        if (rs.success) {
          this.candidateOffers = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(entity: CandidateOffer): void {
    return;
  }

  getCandidateInfo(entity: CandidateOffer) {
    const candidateInfo: CandidateInfo = {
      ...entity,
      id: entity.cvId,
    };
    return candidateInfo;
  }

  getRequisitionInfo(entity: CandidateOffer): RequisitionInfo {
    return {
      ...entity,
      id: entity.requestId,
      requestStatusName: null,
      branchName: entity.requestBranchName,
      subPositionName: entity.subPositionNameRequest
    }
  }

  onPositionSelect(subPositionId: number) {
    this.searchDetail.subPositionId = subPositionId;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  onFinalLevelChange(value: number, entity: CandidateOffer, isIntern: boolean) {
    if (!isIntern) return;
    entity.salary = this._utilities.catInternSalary.find(item => item.id === value).salary;
  }

  onSave(entity: CandidateOffer) {
    const payload: CandidateOfferPayload = {
      id: entity.id,
      finalLevel: entity.finalLevel,
      status: entity.requestCVStatus,
      salary: entity.salary,
      onboardDate:
        entity.onboardDate &&
        moment(entity.onboardDate).format(DateFormat.YYYY_MM_DD),
      hrNote: entity.hrNote,
    };

    this.subs.add(
      this._candidateOffer.update(payload).subscribe((res) => {
        this.isLoading = res.loading;
        if (res.error) {
          this.onCancel(entity);
          return;
        }

        if (!res.loading && res.result && res.success) {
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS,res.result.fullName);
          this.refresh();
        }
      })
    );
  }

  onEdit(entity: CandidateOffer) {
    entity.onboardDate = (entity.onboardDate && new Date(entity.onboardDate)) || null;
    this.clonedCandidateOffer[entity.id] = copyObject(entity);
  }

  onCancel(entity: CandidateOffer) {
    const i = this.candidateOffers.findIndex((item) => item.id === entity.id);
    this.candidateOffers[i] = this.clonedCandidateOffer[entity.id];
  }

  navigateToRequestDetail(candidate: CandidateOffer) {
    this.router.navigate(["/app/requisition", candidate.requestId], {
      queryParams: {
        type: candidate.userType,
      },
    });
  }

  private getPayload(request: PagedRequestDto) {
    const filterItems: Filter[] = [];
    let filterObj: Filter = null;
    for (const property in this.searchDetail) {
      if (checkNumber(this.searchDetail[property])) {
        filterObj = {
          propertyName: property,
          value: this.searchDetail[property],
          comparision: COMPARISION_OPERATOR.Equal,
        };
        filterItems.push(filterObj);
      }
    }
    request.filterItems = filterItems;
    return request;
  }

  private getBreadCrumbConfig() {
    return {
      menuItem: [{ label: "Candidate Offer" }],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

  public updateToHRM(candidateInfo:CandidateOffer){
    this.subs.add(
    this._candidateOffer.UpdateHrmTempEmployee(candidateInfo.id)
    .subscribe(rs => {
      if (!rs.loading && rs.result && rs.success) {
        this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS, "Updated employee to HRM");
        this.refresh();
      }
    })
    )
  }

  public getListItem(candidateInfo:CandidateOffer): MenuItem[] {
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
          this.updateToHRM(candidateInfo);
        },
        visible: candidateInfo.requestCVStatus == REQUEST_CV_STATUS.AcceptedOffer
        || candidateInfo.requestCVStatus == REQUEST_CV_STATUS.RejectedOffer
      }]
    }]
  }
}
