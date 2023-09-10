import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Project } from '@app/modules/ncc-cv/project/list-project/list-project.component';
import { ApiResponse, PagedResult } from '@shared/paged-listing-component-base';
import { Observable, of } from 'rxjs';
import { catchError, map, startWith } from 'rxjs/operators';
import { BaseApiService } from '../apis/base-api.service';

@Injectable({
  providedIn: 'root'
})
export class ProjectService extends BaseApiService {
  changeUrl(): string {
    return 'Project';
  }

  constructor(httpClient: HttpClient) {
    super(httpClient);
  }

  public getAllProject(name: string, type: number): Observable<ApiResponse<Project[]>> {
    return this.get(`/GetAll?type=${type}&name=${name}`);
  }

  public getAllPagingProject(payload): Observable<ApiResponse<PagedResult>> {
    return this.http
      .post<any>(this.rootUrl + "/GetAllPaging", payload)
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err.error.error });
        })
      );
  }

  public save(payload: Project): Observable<ApiResponse<Project>> {
    return this.http
      .post<any>(this.rootUrl + "/Save", payload)
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err.error.error });
        })
      )
  }

  public getUserWorkingInProject(
    projectId: number,
    skipCount: number,
    maxResultCount: number
  ): Observable<ApiResponse<PagedResult>> {
    return this.http
      .get<any>(this.rootUrl + `/ReportOfWorkedInProject?ProjectId=${projectId}&skipCount=${skipCount}&maxResultCount=${maxResultCount}`)
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err.error.error });
        })
      )
  }

  public getUserUsedInProject(
    projectId: number,
    skipCount: number,
    maxResultCount: number
  ): Observable<ApiResponse<PagedResult>> {
    return this.http
      .get<any>(this.rootUrl + `/ReportOfUsedProject?ProjectId=${projectId}&skipCount=${skipCount}&maxResultCount=${maxResultCount}`)
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err.error.error });
        })
      )
  }
}
