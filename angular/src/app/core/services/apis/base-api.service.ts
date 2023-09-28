import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
  HttpParams
} from "@angular/common/http";
import { InjectionToken } from "@angular/core";
import { AppConsts } from "@shared/AppConsts";
import {
  ApiResponse,
  PagedRequestDto,
  PagedResult
} from "@shared/paged-listing-component-base";
import { Observable, of } from "rxjs";
import { catchError, map, startWith } from "rxjs/operators";
export const API_BASE_URL = new InjectionToken<string>("API_BASE_URL");

export abstract class BaseApiService {
  protected baseUrl = AppConsts.remoteServiceBaseUrl;

  get rootUrl() {
    return this.baseUrl + "/api/services/app/" + this.changeUrl();
  }

  constructor(public http: HttpClient) {
    this.http = http;
  }

  abstract changeUrl(): string;

  getAllPagging(request: PagedRequestDto): Observable<ApiResponse<PagedResult>> {
    return this.http
      .post<any>(this.rootUrl + "/GetAllPaging", request)
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err.error.error });
        })
      );
  }

  get(subUrl: string): Observable<ApiResponse<any>> {
    return this.http.get<any>(this.rootUrl + subUrl).pipe(
      map(data => ({ ...data, loading: false })),
      startWith({ loading: true, success: false }),
      catchError((err: HttpErrorResponse) => {
        return of({ loading: false, success: false, error: err.error.error });
      })
    );
  }

  getAll(subUrl: string = null): Observable<ApiResponse<any>> {
    return this.http
      .get<any>(this.rootUrl + (subUrl || "/GetAll"))
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err.error.error });
        })
      );
  }

  public getById(id: any): Observable<ApiResponse<any>> {
    return this.http.get<any>(this.rootUrl + "/Get?id=" + id).pipe(
      map(data => ({ ...data, loading: false })),
      startWith({ loading: true, success: false }),
      catchError((err: HttpErrorResponse) => {
        return of({ loading: false, success: false, error: err.error.error });
      })
    );
  }

  public delete(id: any, subUrl: string = null): Observable<ApiResponse<any>> {
    return this.http
      .delete<any>(this.rootUrl + (subUrl || "/Delete?Id=" + id)).pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err.error.error });
        })
      );
  }

  public update(item: any, subUrl: string = null): Observable<ApiResponse<any>> {
    return this.http.put<any>(this.rootUrl + (subUrl || "/Update"), item).pipe(
      map(data => ({ ...data, loading: false })),
      startWith({ loading: true, success: false }),
      catchError((err: HttpErrorResponse) => {
        return of({ loading: false, success: false, error: err.error.error });
      })
    );
  }

  public create(item: any, subUrl: string = null): Observable<ApiResponse<any>> {
    return this.http
      .post<any>(this.rootUrl + (subUrl || "/Create"), item)
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err.error.error });
        })
      );
  }

  createExport(item: any, subUrl: string = null): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json'
    });
    return this.http.post(this.rootUrl + subUrl, item, {
      headers: headers,
      responseType: 'blob'
    }).pipe(
      catchError((err: HttpErrorResponse) => {
        return of({ loading: false, success: false, error: err.error.error }); // Return an empty Blob in case of error
      })
    );
  }
}
