import { RequisitionPayload, RequisitionInfo } from "./../../../core/models/requisition/requisition.model";
import { Component, Injector, OnInit, Optional } from "@angular/core";
import { Router } from "@angular/router";
import { checkNumber, copyObject } from "@app/core/helpers/utils.helper";
import {
  CandidateIntern,
  CandidatePayloadListData,
} from "@app/core/models/candidate/candidate.model";
import { UtilitiesService } from "@app/core/services/utilities.service";
import { FILTER_TIME, DateFormat, MESSAGE } from "@shared/AppConsts";
import {
  API_RESPONSE_STATUS,
  COMPARISION_OPERATOR,
  CreationTimeEnum,
  SearchType,
  SortType,
  ToastMessageType,
  UserType,
  DefaultRoute,
  CANDIDATE_DETAILT_TAB_DEFAULT,
} from "@shared/AppEnums";
import { TalentDateTime } from "@shared/components/date-selector/date-selector.component";
import {
  Filter,
  PagedListingComponentBase,
  PagedRequestDto,
} from "@shared/paged-listing-component-base";
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from "primeng/dynamicdialog";
import { CandidateInternService } from "./../../../core/services/candidate/candidate-intern.service";
import { RequisitionInternService } from "./../../../core/services/requisition/requisition-intern.service";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { ExportCandidateComponent } from "@shared/components/export-candidate/export-candidate.component";
import { ConfirmPresentForHr } from "@shared/pages/create-candidate/current-requisition/confirm-presentforhr/confirm-presentforhr.component";

@Component({
  selector: "talent-candidate-intern-list",
  templateUrl: "./candidate-intern-list.component.html",
  styleUrls: ["./candidate-intern-list.component.scss"],
})
export class CandidateInternListComponent
  extends PagedListingComponentBase<CandidateIntern>
  implements OnInit
{
  public readonly FILTER_TIME = FILTER_TIME;
  public readonly SORT_TYPE = SortType;
  public readonly DATE_FORMAT = DateFormat;

  isShowAdvanceSearch = false;

  catalogConfig = {
    labelName: "Candidate Skill",
    catalogList: this._utilities.catSkill,
    optionLabel: "name",
    optionValue: "id",
  };

  searchDetail = {
    subPositionId: null,
    branchId: null,
    cvStatus: null,
    creatorUserId: null,
  };

  indexOfLastMail: number;
  searchWithSkills: number[];
  searchWithReqStatusCV: number;
  searchWithProcessCvStatus: number;
  searchWithFromStatus: number;
  searchWithToStatus: number;
  searchWithCreationTime: TalentDateTime;
  isSearchAnd = false;

  isDialogMode = false;
  internIdsInReqList: number[] = [];
  requisitionInternId: number = null;

  candInterns: CandidateIntern[] = [];
  showDialogUpdateNote = false;
  candidateInternEdit: CandidateIntern;

  constructor(
    injector: Injector,
    @Optional() public config: DynamicDialogConfig,
    @Optional() public ref: DynamicDialogRef,
    public _utilities: UtilitiesService,
    public _candidateIntern: CandidateInternService,
    private _reqIntern: RequisitionInternService,
    public _router: Router,
    private _modalService: BsModalService,
    private _dialog: DialogService
  ) {
    super(injector);
    this.route.queryParams.subscribe(params => {
      const statusParam = params['cvStatus'] ? Number(params['cvStatus']) : null;
      const isStatusId = _utilities.catCvStatus.some(status => (status.id === statusParam));
      this.searchDetail.cvStatus = isStatusId && statusParam ? statusParam : null;
    });
  }

  ngOnInit(): void {
    localStorage.removeItem('idApplyCV')
    this.searchText = "";
    this.searchDetail.creatorUserId =
    this._utilities.catCanInternCreatedBy[0]?.id;
    this.isDialogMode = !!this.config?.data?.dialogMode;
    this.requisitionInternId = this.config?.data?.reqInternId;
    this.internIdsInReqList = copyObject(this.config?.data?.selectedIds);
    this.breadcrumbConfig = this.getBreadcrumbConfig();
    this.indexOfLastMail = 3;
  }

  getAllMail(lengthOfListMail: number) {
    if(this.indexOfLastMail === lengthOfListMail){
      return this.indexOfLastMail = 3
    }
    return this.indexOfLastMail = lengthOfListMail;
  }

  getRowspanRequisiton(item: CandidateIntern) {
    if (!item.requisitionInfos || item.requisitionInfos.length === 0) {
      return 1;
    }
    return item.requisitionInfos.length;
  }

  getRequisitionInfo(
    reqInfo: RequisitionInfo,
    candidate: CandidateIntern
  ): RequisitionInfo {
    return { ...reqInfo, userType: candidate.userType };
  }

  getTabActiveDetail(candidate: CandidateIntern) {
    if (candidate.requisitionInfos.length) {
      return CANDIDATE_DETAILT_TAB_DEFAULT.CURRENT_REQ;
    }
    return CANDIDATE_DETAILT_TAB_DEFAULT.PERSONAL_INFO;
  }

  toggleSeachAdvance() {
    this.isShowAdvanceSearch = !this.isShowAdvanceSearch;
  }

  createCandidate() {
    this.router.navigate(["/app/candidate/intern-list/create"], {
      queryParams: { userType: UserType.INTERN },
    });
  }

  onSearchTypeChange(value: string) {
    this.isSearchAnd = value === SearchType.AND;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  onTalentDateChange(talentDateTime: TalentDateTime) {
    this.searchWithCreationTime = talentDateTime;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  onShowEditNote(item: CandidateIntern) {
    this.showDialogUpdateNote = true;
    this.candidateInternEdit = item;
  }

  cancelUpdateNote() {
    this.showDialogUpdateNote = false;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  onCvStatusChanges() {
    this.router.navigate([], {
      queryParamsHandling: "merge",
      replaceUrl: true,
      queryParams: { cvStatus: this.searchDetail.cvStatus }
    }).then(() =>
      this.getDataPage(this.GET_FIRST_PAGE)
    )
  }

  onSaveUpdateNote() {
    const payload = {
      cvId: this.candidateInternEdit.id,
      note: this.candidateInternEdit.note,
    };

    this.subs.add(
      this._candidateIntern.updateCandidateNote(payload).subscribe((res) => {
        this.isLoading = res.loading;
        if (res.success) {
          const index = this.candInterns.findIndex(
            (item) => item.id === payload.cvId
          );
          this.candInterns[index].note = res.result.note;
          this.showToastMessage(
            ToastMessageType.SUCCESS,
            MESSAGE.UPDATE_SUCCESS
          );

          this.showDialogUpdateNote = false;
          this.candidateInternEdit = null;
        }
      })
    );
  }

  onPositionSelect(subPositionId: number) {
    this.searchDetail.subPositionId = subPositionId;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  onSkillChange(event) {
    this.searchWithSkills = event;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  isInRequisition(entity: CandidateIntern): boolean {
    return this.internIdsInReqList.includes(entity.id);
  }

  onReqSeletedCandidate(entity: CandidateIntern) {
    const dialogRef = this._dialog.open(ConfirmPresentForHr, {
      showHeader: false,
      width: '25%',
      contentStyle: { 'background-color': 'rgba(242,245,245)', overflow: 'visible' },
      baseZIndex: 10000,
      data: entity
    });
    dialogRef.onClose.subscribe((ref: { isPresentForHr: boolean}) => {
      if (ref) {
        if(!entity.requisitionInfos[0] &&  ref.isPresentForHr == true){
          this.showToastMessage(
            ToastMessageType.ERROR,
            MESSAGE.ADD_FAILD
          );
          return;
        }
      const payload : RequisitionPayload = {
        cvId: entity.id,
        requestId: this.requisitionInternId,
        currentRequestId: entity.requisitionInfos[0]?.id || null ,
        isPresentForHr: ref.isPresentForHr
      }

        this.subs.add(
          this._reqIntern
            .createRequestCV(payload)
            .subscribe((res) => {
              this.isLoading = res.loading;
              if (!res.loading && res.success) {
                const index = this.candInterns.findIndex((item) => item === entity);
                this.candInterns[index] = res.result.cv;
                this._reqIntern.setCurrentReqIntern(res.result.requisition);
                this.showToastMessage(
                  ToastMessageType.SUCCESS,
                  `Added cancidate ${entity.fullName}`
                );
                this.ref.close(entity)
              }
            })
        );
      }
    });
  }

  protected list(
    request: PagedRequestDto,
    pageNumber: number,
    finishedCallback: Function
  ): void {
    const payload = this.getPayLoad(request);
    this.subs.add(
      this._candidateIntern.getAllPagging(payload).subscribe((rs) => {
        this.candInterns = [];
        if (rs.success) {
          this.candInterns = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(entity: CandidateIntern): void {
    const deleteRequest = this._candidateIntern.delete(entity.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(
        deleteRequest,
        entity.fullName
      ).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  clone(id: number) {
    if (id) {
      this.subs.add(
        this._candidateIntern.cloneCandidateByCvId(id).subscribe((res) => {
          if (res.success) {
            this.showToastMessage(
              ToastMessageType.SUCCESS,
              MESSAGE.CLONE_SUCCESS
            );
            this.getDataPage(this.GET_FIRST_PAGE);
          }
        })
      );
    }
  }

  private getPayLoad(request: PagedRequestDto): CandidatePayloadListData {
    const payload: any = { ...request };
    const filterItems: Filter[] = [];

    payload.isAndCondition = this.isSearchAnd;
    this.searchWithSkills?.length && (payload.skillIds = this.searchWithSkills);
    (this.searchWithReqStatusCV || this.searchWithReqStatusCV === 0) && (payload.requestCVStatus = this.searchWithReqStatusCV);
    (this.searchWithFromStatus || this.searchWithFromStatus === 0) && (payload.fromStatus = this.searchWithFromStatus);
    (this.searchWithToStatus || this.searchWithToStatus === 0) && (payload.toStatus = this.searchWithToStatus);
    (this.searchWithProcessCvStatus || this.searchWithProcessCvStatus === 0) && (payload.processCVStatus = this.searchWithProcessCvStatus);
    if (
      this.searchWithCreationTime &&
      this.searchWithCreationTime.dateType !== CreationTimeEnum.ALL
    ) {
      payload.fromDate = this.searchWithCreationTime?.fromDate?.format(
        DateFormat.YYYY_MM_DD
      );
      payload.toDate = this.searchWithCreationTime?.toDate?.format(
        DateFormat.YYYY_MM_DD
      );
    }

    for (const property in this.searchDetail) {
      if (checkNumber(this.searchDetail[property])) {
        const filterObj = {
          propertyName: property,
          value: this.searchDetail[property],
          comparision: COMPARISION_OPERATOR.Equal,
        };
        filterItems.push(filterObj);
      }
    }

    payload.filterItems = filterItems;
    return payload;
  }

  private getBreadcrumbConfig() {
    return {
      menuItem: [
        {
          label: "Candidate",
          routerLink: DefaultRoute.Candidate,
          styleClass: "menu-item-click",
        },
        { label: "Intern List" },
      ],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }
  onSearchEnter() {
    this.searchDetail.creatorUserId = null;
    this.filterItems = this.filterItems.filter(
      (item) => item.propertyName != "creatorUserId"
    );
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  showExportDialog(userType: number): void {
    let createExportCandidate: BsModalRef;
    userType = UserType.INTERN
    createExportCandidate =  this._modalService.show(
    ExportCandidateComponent, {
      class: 'modal-lg',
      initialState: {
        userType: userType,
      },
    });
  }
}
