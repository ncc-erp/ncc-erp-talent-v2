import {
  HttpClient,
  HttpErrorResponse,
  HttpParams,
} from "@angular/common/http";
import { Injectable } from "@angular/core";
import {
  RequisitionStaffCreateResponse,
  RequisitionStaff,
  RequisitonCandidate,
  RequisitionPagedResult,
} from "@app/core/models/requisition/requisition.model";
import { ApiResponse, PagedRequestDto } from "@shared/paged-listing-component-base";
import { Observable, of } from "rxjs";
import { catchError, map, startWith } from "rxjs/operators";
import { BaseApiService } from "../apis/base-api.service";

@Injectable({
  providedIn: "root",
})
export class RequisitionStaffService extends BaseApiService {

  private currentRequestStaff: RequisitionStaff = null;

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return "RequisitionStaff";
  }

  getAllPagging(request: PagedRequestDto): Observable<ApiResponse<RequisitionPagedResult>> {
    return super.getAllPagging(request) as Observable<ApiResponse<RequisitionPagedResult>>;
  }

  getById(requestId: number): Observable<ApiResponse<RequisitionStaff>> {
    return this.get('/GetById?id=' + requestId);
  }

  closeRequisition(
    requestId: number
  ): Observable<ApiResponse<RequisitionStaff>> {
    return this.get("/CloseRequest?requestId=" + requestId);
  }

  reopenRequisition(
    requestId: number
  ): Observable<ApiResponse<RequisitionStaff>> {
    return this.get("/ReOpenRequest?requestId=" + requestId);
  }

  /**
   * Get list candidate-staff by requisitionId
   * @param requestId: requisition Id
   * @returns list candidate-staff
   */
  getCVsByRequestId(
    requestId: number
  ): Observable<ApiResponse<RequisitonCandidate[]>> {
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

  createRequestCV(requestId: number, cvId: number ): Observable<ApiResponse<RequisitionStaffCreateResponse>> {
    return this.get(`/CreateRequestCV?requestId=${requestId}&cvId=${cvId}`);
  }

  deleteRequestCV(
    requestCvId: number,
    requestId: number
  ): Observable<ApiResponse<RequisitionStaff>> {
    return this.http
      .delete<any>(this.rootUrl + "/DeleteRequestCV", {
        params: new HttpParams().appendAll({
          requestCvId: requestCvId,
          requestId: requestId,
        }),
      })
      .pipe(
        map((data) => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err });
        })
      );
  }

  setCurrentReqStaff(reqIntern: RequisitionStaff) {
    this.currentRequestStaff = reqIntern;
  }

  getCurrentReqStaff(): RequisitionStaff {
    return this.currentRequestStaff;
  }

}
