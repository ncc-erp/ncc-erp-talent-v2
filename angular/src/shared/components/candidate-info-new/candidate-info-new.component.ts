import { Component, Injector, Input, OnInit } from '@angular/core';
import { checkNumber } from '@app/core/helpers/utils.helper';
import { AppComponentBase } from '@shared/app-component-base';
import { CANDIDATE_DETAILT_TAB_DEFAULT, UserType } from '@shared/AppEnums';
import { CandidateInfo } from '../../../app/core/models/candidate/candidate.model';
import { UtilitiesService } from './../../../app/core/services/utilities.service';
import { CustomDialogService } from './../../../app/core/services/custom-dialog/custom-dialog.service';

@Component({
  selector: 'talent-candidate-info-new',
  templateUrl: './candidate-info-new.component.html',
  styleUrls: ['./candidate-info-new.component.scss']
})
export class CandidateInfoNewComponent extends AppComponentBase implements OnInit {

  @Input() data: CandidateInfo;
  @Input() showSkills = true;
  @Input() showCVStatus = true;
  @Input() showUserType = false;
  @Input() isOpenDetail = true;
  @Input() tabActive: CANDIDATE_DETAILT_TAB_DEFAULT = CANDIDATE_DETAILT_TAB_DEFAULT.PERSONAL_INFO;


  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    public customDialogService: CustomDialogService,
  ) {
    super(injector);
  }

  ngOnInit(): void { }

  navigateToCanDetail(slug: number) {
    if(!this.isOpenDetail) return;
    const candidatePath = this.data.userType === UserType.INTERN ? 'intern-list' : 'staff-list';
    const url = this.router.createUrlTree(
      [`/app/candidate/${candidatePath}`, slug],
      { queryParams: { userType: this.data.userType, tab: this.getTabActiveDetail(this.data?.requestId) } }
    );
    window.open(url.toString(), '_blank')
  }
  openPDFDocViewer() {
    this.customDialogService.openPDFDocViewerDialog(this.data.linkCV);
  }

  private getTabActiveDetail(requestId: number) {
    return checkNumber(requestId) ? CANDIDATE_DETAILT_TAB_DEFAULT.CURRENT_REQ : this.tabActive;
  }

}
