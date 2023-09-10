import { PagedRequestDto } from '@shared/paged-listing-component-base';
import { map, startWith, catchError } from 'rxjs/operators';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { BaseApiService } from '../apis/base-api.service';
import { ApiResponse, PagedResult } from './../../../../shared/paged-listing-component-base';
import { RequisitionIntern, RequisitonCandidate, RequisitionCloseAndClone, PayloadRequisition, RequisitionInternCreateResponse, CloseCloneAllRequestInternPayload, RequisitionPagedResult } from './../../models/requisition/requisition.model';

@Injectable({
  providedIn: 'root'
})
export class RequisitionInternService extends BaseApiService {

  private currentRequestIntern: RequisitionIntern = null;

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'RequisitionIntern';
  }

  getAllPagging(request: PagedRequestDto): Observable<ApiResponse<RequisitionPagedResult>> {
    return super.getAllPagging(request) as Observable<ApiResponse<RequisitionPagedResult>>;
  }

  getById(requestId: number): Observable<ApiResponse<RequisitionIntern>> {
    return this.get('/GetById?id=' + requestId);
  }

  closeRequisition(requestId: number): Observable<ApiResponse<RequisitionIntern>> {
    return this.get("/CloseRequest?requestId=" + requestId);
  }

  reopenRequisition(requestId: number): Observable<ApiResponse<RequisitionIntern>> {
    return this.get("/ReOpenRequest?requestId=" + requestId);
  }

  getRequisitionToCloseAndClone(requestId: number): Observable<ApiResponse<RequisitionCloseAndClone>> {
    return this.get("/GetRequisitionToCloseAndClone?requestId=" + requestId);
  }

  /**
   * Get list candidate-staff by requisitionId
   * @param requestId: requisition Id
   * @returns list candidate-staff
   */
  getCVsByRequestId(requestId: number): Observable<ApiResponse<RequisitonCandidate[]>> {
    return this.get("/GetCVsByRequestId?requestId=" + requestId);
  }

  /**
   * Get list id candidate-staff by requisitionId
   * @param requestId: requisition Id
   * @returns list candidate's id in a specific requisition
   */
  getCVIdsByReqquestId(requestId: number): Observable<ApiResponse<number[]>> {
    return this.get("/GetCVIdsByReqquestId?requestId=" + requestId);
  }

  createRequestCV(requestId: number, cvId: number): Observable<ApiResponse<RequisitionInternCreateResponse>> {
    return this.get(`/CreateRequestCV?requestId=${requestId}&cvId=${cvId}`);
  }

  closeAndCloneRequest(payload: PayloadRequisition): Observable<ApiResponse<RequisitionIntern>> {
    return this.create(payload, '/CloseAndCloneRequest');
  }

  deleteRequestCV(requestCvId: number, requestId: number): Observable<ApiResponse<RequisitionIntern>> {
    return this.http
      .delete<any>(this.rootUrl + '/DeleteRequestCV', {
        params: new HttpParams().appendAll({
          requestCvId: requestCvId,
          requestId: requestId
        })
      }).pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err });
        })
      );
  }

  setCurrentReqIntern(reqIntern: RequisitionIntern) {
    this.currentRequestIntern = reqIntern;
  }

  getCurrentReqIntern(): RequisitionIntern {
    return this.currentRequestIntern;
  }

  getAllCloseAndCloneRequisition(): Observable<ApiResponse<RequisitionIntern[]>> {
    return this.getAll("/GetAllCloseAndCloneRequisition");
  }

  closeAndCloneAllRequisition(payload: CloseCloneAllRequestInternPayload): Observable<ApiResponse<PagedResult>> {
    return this.create(payload, "/CloseAndCloneAllRequisition")
  }

}
