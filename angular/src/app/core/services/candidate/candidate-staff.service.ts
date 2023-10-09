import { MailDetail } from './../../models/candidate/candidate.model';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CandidateApplyResult, CandidateApplyResultPayload, CandidateInterviewLevel, CandidateInterviewLevelPayload, CandidateInterviewed, CandidateInterviewedPayload } from '@app/core/models/candidate/candiadte-requisition.model';
import { CandidateApplyHistory } from '@app/core/models/candidate/candidate-apply-history.model';
import { CandidateEducation, CandidateEducationPayload } from '@app/core/models/candidate/candidate-education.model';
import { CandidatePayload, CandidateStaff } from '@app/core/models/candidate/candidate.model';
import { MailPreviewInfo } from '@app/core/models/mail/mail.model';
import { BehaviorSubject, Observable } from 'rxjs';
import { BaseApiService } from '../apis/base-api.service';
import { ApiResponse } from './../../../../shared/paged-listing-component-base';
import { CandidateCapability, CandidateRequisiton } from './../../models/candidate/candiadte-requisition.model';
import { CandidateSkill, CandidateSkillPayload } from './../../models/candidate/candidate-skill.model';
import { CatalogModel } from './../../models/common/common.dto';
import {UserType} from '@shared/AppEnums';

@Injectable({
  providedIn: 'root'
})
export class CandidateStaffService extends BaseApiService {

  private currentReqUpdated$ = new BehaviorSubject<boolean>(false);
  private fragment$ = new BehaviorSubject<{ tab: number, fragment: string }>(null);

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'CandidateStaff';
  }

  getCurrentReqUpdated$(): Observable<boolean> {
    return this.currentReqUpdated$.asObservable();
  }

  getFragment$(): Observable<{ tab: number, fragment: string }> {
    return this.fragment$.asObservable();
  }

  setCurrentReqUpdated(value: boolean) {
    this.currentReqUpdated$.next(true);
  }

  setFragment(value: { tab: number, fragment: string }) {
    this.fragment$.next(value);
  }

  getById(candidateId: number): Observable<ApiResponse<CandidateStaff>> {
    return this.get('/GetCVById?cvId=' + candidateId);
  }

  getCanEducationById(candidateId: number): Observable<ApiResponse<CandidateEducation[]>> {
    return this.get('/GetEducationCVsByCVId?cvId=' + candidateId);
  }

  getCanSkillById(candidateId: number): Observable<ApiResponse<CandidateSkill[]>> {
    return this.get('/GetSkillCVsByCVId?cvId=' + candidateId);
  }

  getCurentReqByCanId(candidateId: number): Observable<ApiResponse<CandidateRequisiton>> {
    return this.get('/GetCurrentRequisitionByCVId?cvId=' + candidateId);
  }

  getApplyHistory(candidateId: number): Observable<ApiResponse<CandidateApplyHistory[]>> {
    return this.get("/GetHistoryCV?cvId=" + candidateId);
  }

  getAllUserCreated(): Observable<ApiResponse<CatalogModel[]>> {
    return this.getAll('/GetAllUserCreated');
  }

  getPreviewMailFailed(cvId: number): Observable<ApiResponse<MailPreviewInfo>> {
    return this.get("/PreviewBeforeSendMailCV?cvId=" + cvId);
  }

  getPreviewRequestCvMail(requestCvId: number): Observable<ApiResponse<MailPreviewInfo>> {
    return this.get("/PreviewBeforeSendMailRequestCV?requestCVId=" + requestCvId);
  }

  createEducation(payload: CandidateEducationPayload): Observable<ApiResponse<CandidateEducation>> {
    return this.create(payload, '/CreateEducationCV');
  }

  createSkill(payload: CandidateSkillPayload): Observable<ApiResponse<CandidateSkill>> {
    return this.create(payload, '/CreateCVSkill');
  }

  createReqCV(payload: { cvId: number, requestId: number }): Observable<ApiResponse<CandidateRequisiton>> {
    return this.create(payload, '/CreateRequestCV');
  }

  addInterviewer(payload: { interviewerId: number, requestCvId: number }) {
    return this.create(payload, '/AddInterviewerInCVRequest');
  }

  sendMailCV(payload: MailPreviewInfo, cvId: number): Observable<ApiResponse<MailDetail>> {
    return this.create(payload, `/SendMailCV?cvId=${cvId}`);
  }

  sendMailRequestCV(payload: MailPreviewInfo, requestCvId: number): Observable<ApiResponse<MailDetail>> {
    return this.create(payload, `/SendMailRequestCV?requestCVId=${requestCvId}`);
  }

  updateCV(payload: CandidatePayload): Observable<ApiResponse<CandidateStaff>> {
    return this.update(payload, '/UpdateCV');
  }

  updateFileCV(payload: FormData): Observable<ApiResponse<string>> {
    return this.create(payload, '/UpdateFileCV');
  }

  updateFileAvatar(payload: FormData): Observable<ApiResponse<string>> {
    return this.create(payload, '/UpdateFileAvatar');
  }

  updateInterviewTime(payload: { requestCVId: number, interviewTime: string }) {
    return this.update(payload, '/UpdateInterviewTime');
  }

  updateCvSkill(payload: CandidateSkillPayload): Observable<ApiResponse<CandidateSkill>> {
    return this.update(payload, '/UpdateSkillCV');
  }

  updateCapabilityCV(payload: CandidateCapability): Observable<ApiResponse<CandidateCapability>> {
    return this.update(payload, '/UpdateCapabilityCV');
  }

  updateManyCapabilityCV(payload: CandidateCapability[]): Observable<ApiResponse<CandidateCapability[]>> {
    return this.update(payload, '/UpdateManyCapabilityCV');
  }

  updateManyFactorsCapabilityCV(payload: CandidateCapability[]): Observable<ApiResponse<CandidateCapability[]>> {
    return this.update(payload, '/UpdateManyFatorsCapabilityCV');
  }

  updateApplicationResult(payload: CandidateApplyResultPayload): Observable<ApiResponse<CandidateApplyResult>> {
    return this.update(payload, '/UpdateApplicationResult');
  }

  updateInterviewLevel(payload: CandidateInterviewLevelPayload): Observable<ApiResponse<CandidateInterviewLevel>> {
    return this.update(payload, '/UpdateInterviewLevel');
  }

  updateInterviewed(payload: CandidateInterviewedPayload): Observable<ApiResponse<CandidateInterviewed>> {
    return this.update(payload, '/UpdateInterviewed');
  }

  deleteEducationCV(id: number): Observable<ApiResponse<string>> {
    return this.delete(id, '/DeleteEducationCV?id=' + id)
  }

  deleteSkillCV(id: number): Observable<ApiResponse<string>> {
    return this.delete(id, '/DeleteSkillCV?id=' + id)
  }

  deleteReqCV(requestCvId: number): Observable<ApiResponse<string>> {
    return this.delete(requestCvId, '/DeleteRequestCV?requestCvId=' + requestCvId)
  }

  deleteRequestCVInterview(id: number): Observable<ApiResponse<string>> {
    return this.delete(id, '/DeleteRequestCVInterview?id=' + id)
  }

  /** Return candidateId that contain email */
  checkValidMail(cvId: number, email: string): Observable<ApiResponse<{ cvId: number, userType: number }>> {
    return cvId ? this.get(`/ValidEmail?email=${email}&cvId=${cvId}`) : this.get("/ValidEmail?email=" + email);
  }

  /** Return candidateId that contain phone */
  checkValidPhone(cvId: number, phone: string): Observable<ApiResponse<{ cvId: number, userType: number }>> {
    return cvId ? this.get(`/ValidPhone?phone=${phone}&cvId=${cvId}`) : this.get("/ValidPhone?phone=" + phone);
  }

  createLMSAccount(cvId: number, requestCvId: number): Observable<ApiResponse<string>> {
    return this.get(`/CreateAccountStudent?cvId=${cvId}&requestCVId=${requestCvId}`);
  }

  updateCandidateNote(payload: { cvId: number, note: string }): Observable<ApiResponse<{ cvId: number, note: string }>> {
    return this.create(payload, `/UpdateCandidateNote`);
  }

  cloneCandidateByCvId(id: number): Observable<ApiResponse<string>> {
    return this.get(`/CloneCandidateByCvId?cvId=${id}`);
  }
  
  exportInfomation(payload: {userType: UserType,fromDate:string,toDate:string }): Observable<Blob> {
    return this.generateExport(payload, '/ExportInfo');
  }

  exportReport(payload: {userType: UserType,fromDate:string,toDate:string }): Observable<Blob> {
    return this.generateExport(payload, '/ExportOnboard');
  }
}
