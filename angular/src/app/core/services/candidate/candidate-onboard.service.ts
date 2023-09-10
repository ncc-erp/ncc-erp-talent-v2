import { ApiResponse } from './../../../../shared/paged-listing-component-base';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from '../apis/base-api.service';

@Injectable({
  providedIn: 'root'
})
export class CandidateOnboardService extends BaseApiService {

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'CandidateOnboard';
  }

  public UpdateHrmTempEmployee(requestCvId:number): Observable<ApiResponse<any>> {
    return this.get(`/UpdateHrmTempEmployee?requestCvId=${requestCvId}`);
  }
}
