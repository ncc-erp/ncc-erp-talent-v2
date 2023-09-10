import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResponse, PagedResult } from '../../../../shared/paged-listing-component-base';
import { Observable, of } from 'rxjs';
import { catchError, map, startWith } from 'rxjs/operators';
import { BaseApiService } from '../apis/base-api.service';
import { WorkingExperience } from '@app/core/models/employee-profile/profile-model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeProfileService extends BaseApiService {

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'Employee';
  }

  getAllEmployeePaging(data: Object): Observable<ApiResponse<PagedResult>> {
    return this.create(data, '/GetAllEmployeePaging');
  }

  public getWorkingExperiencePaging(payload): Observable<ApiResponse<PagedResult>> {
    return this.http
      .post<any>(this.rootUrl + "/GetWorkingExperiencePaging", payload)
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err.error.error });
        })
      );
  }

  public updateWorkingExperience(payload: WorkingExperience): Observable<ApiResponse<WorkingExperience>> {
    return this.http
      .post<any>(this.rootUrl + "/UpdateWorkingExperience", payload)
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err.error.error });
        })
      )
  }

  public getEmployeeVersFilter(versionName: string, versionPositionId: number, versionLanguageId: number): Observable<ApiResponse<any>> {
    return this.get(`/GetEmployeeVersFilter?versionName=${versionName}&versionPositionId=${versionPositionId}&versionLanguageId=${versionLanguageId}`);
  }
}
