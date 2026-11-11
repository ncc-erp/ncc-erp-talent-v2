import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnglishInterviewResult } from '@app/core/models/candidate/english-interview.model';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { Observable } from 'rxjs';
import { BaseApiService } from '../apis/base-api.service';

@Injectable({ providedIn: 'root' })
export class EnglishInterviewService extends BaseApiService {
  constructor(public http: HttpClient) {
    super(http);
  }

  changeUrl(): string {
    return 'EnglishInterview';
  }

  getByCVId(cvId: number): Observable<ApiResponse<EnglishInterviewResult[]>> {
    return this.get('/GetByCVId?cvId=' + cvId);
  }
}
