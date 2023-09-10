import { Component, Injector, Input, OnInit } from '@angular/core';
import { CandidateApplyHistory } from '@app/core/models/candidate/candidate-apply-history.model';
import { RequisitionInfo } from '@app/core/models/requisition/requisition.model';
import { CandidateInternService } from '@app/core/services/candidate/candidate-intern.service';
import { CandidateStaffService } from '@app/core/services/candidate/candidate-staff.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { AppComponentBase } from '@shared/app-component-base';
import { DateFormat } from '@shared/AppConsts';
import { SortType } from '@shared/AppEnums';
import { UserType } from './../../../AppEnums';

@Component({
  selector: 'talent-application-history',
  templateUrl: './application-history.component.html',
  styleUrls: ['./application-history.component.scss']
})
export class ApplicationHistoryComponent extends AppComponentBase implements OnInit {

  @Input() userType: number;
  @Input() candidateId: number;
  @Input() _candidate: CandidateInternService | CandidateStaffService;

  historyReqs: CandidateApplyHistory[] = [];


  public readonly SORT_TYPE = SortType;
  public readonly INTERN = UserType.INTERN;
  public readonly DATE_FORMAT = DateFormat;

  constructor(
    private injector: Injector,
    public _utilities: UtilitiesService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this._candidate.getCurrentReqUpdated$().subscribe(res => {
      if(res) {
        this.getCandidateApplyHistory();
      }
    });
    this.getCandidateApplyHistory();
  }

  getRequisitionInfo(entity: CandidateApplyHistory): RequisitionInfo {
    return { ...entity, id: entity.requestId };
  }


  private getCandidateApplyHistory() {
    this.subs.add(
      this._candidate.getApplyHistory(this.candidateId).subscribe(rs => {
        this.historyReqs = [];
        this.isLoading = rs.loading;
        if (rs.success && rs.result) {
          this.historyReqs = rs.result;
        }
      })
    );
  }

}
