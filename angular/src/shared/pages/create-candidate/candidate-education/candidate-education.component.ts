import { Component, Injector, Input, OnInit } from '@angular/core';
import { CandidateEducation, CandidateEducationPayload } from '@app/core/models/candidate/candidate-education.model';
import { Education } from '@app/core/models/categories/education.model';
import { CandidateInternService } from '@app/core/services/candidate/candidate-intern.service';
import { CandidateStaffService } from '@app/core/services/candidate/candidate-staff.service';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { EducationComponent } from '@app/modules/category/education/education.component';
import { AppComponentBase } from '@shared/app-component-base';
import { DateFormat } from '@shared/AppConsts';
import { ActionEnum, API_RESPONSE_STATUS } from '@shared/AppEnums';
import { DialogService } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-candidate-education',
  templateUrl: './candidate-education.component.html',
  styleUrls: ['./candidate-education.component.scss']
})
export class CandidateEducationComponent extends AppComponentBase implements OnInit {

  @Input() userType: number;
  @Input() candidateId: number;
  @Input() _candidate: CandidateInternService | CandidateStaffService;
  @Input() isViewMode: boolean;

  readonly DATE_FORMAT = DateFormat;

  candidateEdus: CandidateEducation[] = [];

  constructor(
    injector: Injector,
    public dialogService: DialogService,
    public _utilities: UtilitiesService,
  ) {
    super(injector);
  }

  ngOnInit(): void {
    if (this.isViewMode) {
      this.getCanEducationData();
    }
  }

  delete(entity: CandidateEducation) {
    const deleteRequest = this._candidate.deleteEducationCV(entity.id);
    this.subs.add(
      this.deleteConfirmAndShowToastMessage(deleteRequest, entity.educationName).subscribe((message) => {
        if (message === API_RESPONSE_STATUS.SUCCESS) {
          this.candidateEdus = this.candidateEdus.filter(item => item.id !== entity.id);
        }
      })
    );
  }

  addEducation() {
    const eduSeletedIds = this.candidateEdus.map(item => item.educationId);
    const canEducationPayload: CandidateEducationPayload = {
      id: 0,
      cvId: this.candidateId,
      educationId: null
    }
    const dialogRef = this.dialogService.open(EducationComponent, {
      header: `Select Education`,
      width: "80%",
      contentStyle: { "max-height": "100%", overflow: "visible" },
      baseZIndex: 10000,
      data: { dialogMode: ActionEnum.SELECT, eduSeletedIds, canEducationPayload, _candidate: this._candidate },
    });

    dialogRef.onClose.subscribe((education: Education) => {
      this.getCanEducationData();
    });
  }

  private getCanEducationData() {
    this.subs.add(
      this._candidate.getCanEducationById(this.candidateId).subscribe(rs => {
        this.candidateEdus = [];
        this.isLoading = rs.loading;
        if (rs.success) {
          this.candidateEdus = rs.result;
        }
      })
    );
  }

}
