import {
  CatalogModel,
  InternSalaryCatalog,
  LevelInfo,
  MailTemplateCatalog,
  PositionCatalog,
} from "./../models/common/common.dto";
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ApiResponse } from "@shared/paged-listing-component-base";
import { Observable } from "rxjs";
import { BaseApiService } from "./apis/base-api.service";
import { MailFunc } from "@shared/AppEnums";

@Injectable({
  providedIn: "root",
})
export class CommonService extends BaseApiService {
  constructor(public http: HttpClient) {
    super(http);
  }

  changeUrl(): string {
    return "Common";
  }

  getRequestStatus() {
    return this.getAll("/GetRequestStatus");
  }

  getRequestCVStatus() {
    return this.getAll("/GetRequestCVStatus");
  }

  getStatusCandidateOnboard() {
    return this.getAll("/GetStatusCandidateOnboard");
  }

  getStatusCandidateOffer() {
    return this.getAll("/GetStatusCandidateOffer");
  }

  getPriority() {
    return this.getAll("/GetPriority");
  }

  getLevel() {
    return this.getAll("/GetLevel");
  }

  getRequestLevel() {
    return this.getAll("/GetRequestLevel");
  }

  getAllUser(): Observable<ApiResponse<CatalogModel[]>> {
    return this.getAll("/GetAllUser");
  }

  getInternSalary(): Observable<ApiResponse<InternSalaryCatalog[]>> {
    return this.getAll("/GetInternSalary");
  }

  getLevelStaff(): Observable<ApiResponse<LevelInfo[]>> {
    return this.getAll("/GetLevelStaff");
  }

  getListCVStatus(): Observable<ApiResponse<CatalogModel[]>> {
    return this.getAll("/GetListCVStatus");
  }

  getListInterviewStatus(): Observable<ApiResponse<CatalogModel[]>> {
    return this.getAll("/GetListInterviewStatus");
  }

  getListCVSourceReferenceType(): Observable<ApiResponse<CatalogModel[]>> {
    return this.getAll("/GetListCVSourceReferenceType");
  }

  getDropdownPositions(): Observable<ApiResponse<PositionCatalog[]>> {
    return this.getAll("/GetDropdownPositions");
  }

  getLevelFinalStaff(): Observable<ApiResponse<LevelInfo[]>> {
    return this.getAll("/GetLevelFinalStaff");
  }

  getLevelFinalIntern(): Observable<ApiResponse<InternSalaryCatalog[]>> {
    return this.getAll("/GetLevelFinalIntern");
  }

  getEmailTemplate(mailFunc: MailFunc = null): Observable<ApiResponse<MailTemplateCatalog[]>> {
    if (mailFunc) {
      return this.getAll(`/GetEmailTemplate?mailFunc=${mailFunc}`);
    } else {
      return this.getAll("/GetEmailTemplate");
    }
  }
}
