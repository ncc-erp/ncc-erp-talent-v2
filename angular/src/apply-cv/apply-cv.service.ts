import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from '@app/core/services/apis/base-api.service';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { ApplyCv } from './apply-cv.model';

import { HttpErrorResponse } from "@angular/common/http";
import { InjectionToken } from "@angular/core";
import { of } from "rxjs";
import { catchError, map, startWith } from "rxjs/operators";
export const API_BASE_URL = new InjectionToken<string>("API_BASE_URL");

@Injectable({
  providedIn: 'root'
})
export class ApplyCvService extends BaseApiService{

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'Public';
  }
  
  public create (payload: any): Observable<ApiResponse<ApplyCv>> {
    return this.http.post<any>(this.rootUrl + "/CreateApplyCV", payload)
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err.error.error });
        })
      );
  }
}
