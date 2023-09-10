import { ApiResponse } from './../../../../shared/paged-listing-component-base';
import { Observable } from 'rxjs';
import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";

import { BaseApiService } from "../apis/base-api.service";

@Injectable({
  providedIn: "root",
})
export class CandidateOfferService extends BaseApiService {
  constructor(public http: HttpClient) {
    super(http);
  }

  changeUrl(): string {
    return "CandidateOffer";
  }

  public UpdateHrmTempEmployee(requestCvId:number): Observable<ApiResponse<any>> {
    return this.get(`/UpdateHrmTempEmployee?requestCvId=${requestCvId}`);
  }
}
