import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { checkNumber } from '@app/core/helpers/utils.helper';
import { Candidate } from '@app/core/models/candidate/candidate.model';
import { CandidateInternService } from '@app/core/services/candidate/candidate-intern.service';
import { CandidateStaffService } from '@app/core/services/candidate/candidate-staff.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { CANDIDATE_DETAILT_TAB_DEFAULT, UserType } from '@shared/AppEnums';

@Component({
  selector: 'talent-create-candidate',
  templateUrl: './create-candidate.component.html',
  styleUrls: ['./create-candidate.component.scss']
})
export class CreateCandidateComponent extends AppComponentBase implements OnInit, OnDestroy {

  tabActived;
  isViewMode: boolean;
  candidateId: number;
  userType: number;
  candidate: Candidate = null;

  isCandidateIntern = false;

  _candidate: CandidateInternService | CandidateStaffService;

  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    private _canStaff: CandidateStaffService,
    private _canIntern: CandidateInternService,
  ) {
    super(injector);
    this.getRouterData();
  }

  ngOnInit(): void {
    this.breadcrumbConfig = this.getBreadcrumbConfig();
    if (this.isViewMode) {
      this.getCandidateInfoData();
    }
  }

  ngOnDestroy(): void {
    this._candidate.setFragment(null);
  }

  activeChange(tabIndex: number) {
    this.tabActived = tabIndex;
  }

  updateQueryTabIndex(tabIndex: number) {
    if(tabIndex === 0){
      this.isViewMode = true
      this.getCandidateInfoData();
    }
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { tab: tabIndex },
      queryParamsHandling: 'merge'
    });
  }

  getThumnailImage() {
    if (this.candidate.avatar) {
      // Not saved yet
      if (this.candidate.avatar.includes('data:image')) return this.candidate.avatar;
      return this._utilities.getLinkFile(this.candidate.avatar);
    }
    return this.candidate.isFemale ? 'assets/img/user-female-circle.png' : 'assets/img/user-circle.png'
  }

  onCandidateChange(candidate: Candidate) {
    if (candidate.branchId) {
      candidate.branchName = this._utilities.catBranch.find(item => item.id === candidate.branchId).name;
    }
    this.candidate = candidate;
  }

  hasCandidate(): boolean {
    return checkNumber(this.candidateId);
  }

  onCreateCandidate(cvId: number) {
    this.candidateId = cvId;
  }

  private getCandidateInfoData() {
    this.subs.add(
      this._candidate.getById(this.candidateId).subscribe(res => {
        this.isLoading = res.loading;
        if (!res.loading && res.success && res.result) {
          this.candidate = res.result;
        }
      })
    );
  }

  private getBreadcrumbConfig() {
    const path = this.isCandidateIntern ? 'intern-list' : 'staff-list';
    const candidateType = this.isCandidateIntern ? 'Intern' : 'Staff';
    return {
      menuItem: [
        { label: `Candidate ${candidateType}`, routerLink: `/app/candidate/${path}`, styleClass: 'menu-item-click' },
        { label: "", disabled: true }
      ],
      homeItem: { icon: "pi pi-home", routerLink: "/" },
    };
  }

  private getRouterData() {
    this.userType = Number(this.route.snapshot.queryParams?.userType);
    this.candidateId = checkNumber(+this.route.snapshot.params?.id) ? +this.route.snapshot.params?.id : null;
    this.isViewMode = !!this.candidateId;
    this.isCandidateIntern = this.userType === UserType.INTERN;
    this._candidate = this.isCandidateIntern ? this._canIntern : this._canStaff;
    this.tabActived = Number(this.route.snapshot.queryParams?.tab) || CANDIDATE_DETAILT_TAB_DEFAULT.PERSONAL_INFO;
  }

}
