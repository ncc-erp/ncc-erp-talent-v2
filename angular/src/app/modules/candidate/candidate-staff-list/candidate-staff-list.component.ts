import { CandidatePayloadListData } from './../../../core/models/candidate/candidate.model';
import { checkNumber } from '@app/core/helpers/utils.helper';
import { Component, Injector, OnInit, Optional } from '@angular/core';
import { CandidateStaffService } from '@app/core/services/candidate/candidate-staff.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { FILTER_TIME, DateFormat, MESSAGE } from '@shared/AppConsts';
import { API_RESPONSE_STATUS, COMPARISION_OPERATOR, CreationTimeEnum, DefaultRoute, SearchType, SortType, ToastMessageType, UserType, CANDIDATE_DETAILT_TAB_DEFAULT } from '@shared/AppEnums';
import { TalentDateTime } from '@shared/components/date-selector/date-selector.component';
import { Filter, PagedListingComponentBase, PagedRequestDto } from '@shared/paged-listing-component-base';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CandidateStaff } from '../../../core/models/candidate/candidate.model';
import { RequisitionStaffService } from '@app/core/services/requisition/requisition-staff.service';
import {BsModalRef, BsModalService} from 'ngx-bootstrap/modal';
import {ExportCandidateComponent} from '@shared/components/export-candidate/export-candidate.component';

@Component({
  selector: 'talent-candidate-staff-list',
  templateUrl: './candidate-staff-list.component.html',
  styleUrls: ['./candidate-staff-list.component.scss']
})
export class CandidateStaffListComponent extends PagedListingComponentBase<CandidateStaff> implements OnInit {

  public readonly FILTER_TIME = FILTER_TIME;
  public readonly SORT_TYPE = SortType;
  public readonly DATE_FORMAT = DateFormat;

  isShowAdvanceSearch = false;

  catalogConfig = {
    labelName: 'Candidate Skill',
    catalogList: this._utilities.catSkill,
    optionLabel: 'name',
    optionValue: 'id'
  }

  searchDetail = {
    subPositionId: null,
    branchId: null,
    cvStatus: null,
    creatorUserId: null,
  }
  indexOfLastMail: number;
  searchWithSkills: number[];
  searchWithReqStatusCV: number;
  searchWithFromStatus: number;
  searchWithToStatus: number;
  searchWithCreationTime: TalentDateTime;
  isSearchAnd = false;

  isDialogMode = false;
  staffIdsInReqList: number[] = [];
  requisitionStaffId: number;

  candStaffs: CandidateStaff[] = [];
  showDialogUpdateNote = false;
  candidateStaffEdit: CandidateStaff;
  searchWithProcessCvStatus: number;
  constructor(
    injector: Injector,
    @Optional() public config: DynamicDialogConfig,
    @Optional() public ref: DynamicDialogRef,
    public _utilities: UtilitiesService,
    public _candidateStaff: CandidateStaffService,
    private _reqStaff: RequisitionStaffService,
    private _modalService: BsModalService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    localStorage.removeItem('idApplyCV')
    this.searchText = "";
    this.searchDetail.creatorUserId = this._utilities.catCanStaffCreatedBy[0]?.id;
    this.isDialogMode = !!this.config?.data?.dialogMode;
    this.requisitionStaffId = this.config?.data?.reqStaffId;
    this.staffIdsInReqList = this.config?.data?.selectedIds;
    this.breadcrumbConfig = this.getBreadcrumbConfig();
    this.indexOfLastMail = 3;
  }

  getAllMail(lengthOfListMail: number) {
    if(this.indexOfLastMail == lengthOfListMail){
      return this.indexOfLastMail = 3
    }
    return this.indexOfLastMail = lengthOfListMail;
  }

  getRowspanRequisiton(item: CandidateStaff) {
    if (!item.requisitionInfos || item.requisitionInfos.length === 0) {
      return 1;
    }
    return item.requisitionInfos.length;
  }

  toggleSeachAdvance() {
    this.isShowAdvanceSearch = !this.isShowAdvanceSearch;
  }

  createCandidate() {
    this.router.navigate(['/app/candidate/staff-list/create'], {
      queryParams: { userType: UserType.STAFF }
    })
  }

  onSearchTypeChange(value: string) {
    this.isSearchAnd = value === SearchType.AND;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  onTalentDateChange(talentDateTime: TalentDateTime) {
    this.searchWithCreationTime = talentDateTime;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  onShowEditNote(item: CandidateStaff) {
    this.showDialogUpdateNote = true;
    this.candidateStaffEdit = item;
  }

  cancelUpdateNote() {
    this.showDialogUpdateNote = false;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  onSaveUpdateNote() {
    const payload = {
      cvId: this.candidateStaffEdit.id,
      note: this.candidateStaffEdit.note
    }

    this.subs.add(
      this._candidateStaff.updateCandidateNote(payload).subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          const index = this.candStaffs.findIndex(item => item.id === payload.cvId);
          this.candStaffs[index].note = res.result.note;
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.UPDATE_SUCCESS);

          this.showDialogUpdateNote = false;
          this.candidateStaffEdit = null;
        }
      })
    );
  }

  getTabActiveDetail(candidate: CandidateStaff) {
    if (candidate.requisitionInfos.length) {
      return CANDIDATE_DETAILT_TAB_DEFAULT.CURRENT_REQ;
    }
    return CANDIDATE_DETAILT_TAB_DEFAULT.PERSONAL_INFO;
  }

  onPositionSelect(subPositionId: number) {
    this.searchDetail.subPositionId = subPositionId;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  onSkillChange(event) {
    this.searchWithSkills = event;
    this.getDataPage(this.GET_FIRST_PAGE);
  }

  isInRequisition(entity: CandidateStaff): boolean {
    return this.staffIdsInReqList.includes(entity.id)
  }

  onReqSeletedCandidate(entity: CandidateStaff) {
    this.subs.add(
      this._reqStaff.createRequestCV(this.requisitionStaffId, entity.id,entity.requisitionInfos[0].id).subscribe(res => {
        this.isLoading = res.loading;
        if (!res.loading && res.success) {
          const index = this.candStaffs.findIndex(item => item === entity);
          this.candStaffs[index] = res.result.cv;
          this._reqStaff.setCurrentReqStaff(res.result.requisition);
          this.showToastMessage(ToastMessageType.SUCCESS, `Added cancidate ${entity.fullName}`);
        }
      })
    );
  }

  protected list(request: PagedRequestDto, pageNumber: number, finishedCallback: Function): void {
    const payload = this.getPayLoad(request);
    this.subs.add(
      this._candidateStaff.getAllPagging(payload).subscribe((rs) => {
        this.candStaffs = [];
        if (rs.success) {
          this.candStaffs = rs.result.items;
          this.showPaging(rs.result, pageNumber);
        }
        this.isLoading = rs.loading;
      })
    );
  }

  protected delete(entity: CandidateStaff): void {
    const deleteRequest = this._candidateStaff.delete(entity.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, entity.fullName).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.refresh();
        }
      })
    );
  }

  clone(id: number) {
    if (id) {
      this.subs.add(
      this._candidateStaff.cloneCandidateByCvId(id).subscribe((res) => {
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
    const payload: any = { ...request }
    const filterItems: Filter[] = [];

    payload.isAndCondition = this.isSearchAnd;
    this.searchWithSkills?.length && (payload.skillIds = this.searchWithSkills);
    (this.searchWithReqStatusCV || this.searchWithReqStatusCV === 0) && (payload.requestCVStatus = this.searchWithReqStatusCV);
    (this.searchWithFromStatus || this.searchWithFromStatus === 0) && (payload.fromStatus = this.searchWithFromStatus);
    (this.searchWithToStatus || this.searchWithToStatus === 0) && (payload.toStatus = this.searchWithToStatus);
    (this.searchWithProcessCvStatus || this.searchWithProcessCvStatus === 0) && (payload.processCVStatus = this.searchWithProcessCvStatus);
    
    if (this.searchWithCreationTime && this.searchWithCreationTime.dateType !== CreationTimeEnum.ALL) {
      payload.fromDate = this.searchWithCreationTime?.fromDate?.format(DateFormat.YYYY_MM_DD);
      payload.toDate = this.searchWithCreationTime?.toDate?.format(DateFormat.YYYY_MM_DD);
    }

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

  private getBreadcrumbConfig() {
    return {
      menuItem: [{ label: "Candidate", routerLink: DefaultRoute.Candidate, styleClass: 'menu-item-click' }, { label: "Staff List" }],
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
   userType = UserType.STAFF
   createExportCandidate =  this._modalService.show(
    ExportCandidateComponent, {
      class: 'modal-lg',
      initialState: {
      userType: userType,
      }, 
    });
  }
}
