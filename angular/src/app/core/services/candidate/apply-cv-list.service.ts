import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BaseApiService } from '../apis/base-api.service';
import {BehaviorSubject, Observable} from 'rxjs';
import {ApiResponse} from '@shared/paged-listing-component-base';
import {ApplyCVPayload, ApplyCv} from '@app/core/models/apply-cv/apply-cv-list.model';
import {CandidateRequisiton} from '@app/core/models/candidate/candiadte-requisition.model';
import {CandidateEducationPayload,CandidateEducation} from '@app/core/models/candidate/candidate-education.model';
import {CandidateSkillPayload,CandidateSkill} from '@app/core/models/candidate/candidate-skill.model';
import {MailDetail} from '@app/core/models/candidate/candidate.model';
import {MailPreviewInfo} from '@app/core/models/mail/mail.model';

@Injectable({
  providedIn: 'root'
})

export class ApplyCVService extends BaseApiService {
  private fragment$ = new BehaviorSubject<{ tab: number, fragment: string }>(null);
  constructor(
    public http: HttpClient
  ) {
    super(http);
  }
  changeUrl(): string {
    return 'ApplyCV';
  }
  setFragment(value: { tab: number, fragment: string }) {
    this.fragment$.next(value);
  }
  getApplyById(applycvId: number): Observable<ApiResponse<ApplyCv>> {
    return this.get('/GetCVById?cvId=' + applycvId);
  }
}
