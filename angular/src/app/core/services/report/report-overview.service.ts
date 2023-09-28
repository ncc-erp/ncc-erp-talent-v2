import { UserType } from '@shared/AppEnums';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { Observable, of } from 'rxjs';
import { BaseApiService } from '../apis/base-api.service';
import { ReportOverview } from '../../models/report/report.model';
import { catchError, map, startWith } from 'rxjs/operators';
import * as moment from 'moment';
import { SourceStatistic } from '@app/core/models/report/report-performance.model';

@Injectable({
  providedIn: 'root'
})
export class ReportOverviewService extends BaseApiService {
  changeUrl(): string {
    return 'Report';
  }

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  public getOverviewStatistic(fd: string, td: string, branchId?: any, userType?: any): Observable<ApiResponse<ReportOverview>> {
    branchId = branchId == null ? '' : branchId;
    userType = userType == null ? '' : userType;
    return this.http.get<any>(this.rootUrl + `/GetOverviewHiring?fd=${fd}&td=${td}&userType=${userType}&branchId=${branchId}`).pipe(
      catchError((err: HttpErrorResponse) => {
        return of({ error: err });
      })
    );
  }
  public getPerformanceStatisticStaff(fd: string, td: string, branchId?: any): Observable<ApiResponse<SourceStatistic>> {
    branchId = branchId == null ? '' : branchId;
    return this.http.get<any>(this.rootUrl + `/GetPerformanceStaffCVSource?fd=${fd}&td=${td}&branchId=${branchId}`).pipe(
      map(data => ({ ...data, loading: false })),
      startWith({ loading: true, success: false }),
      catchError((err: HttpErrorResponse) => {
        return of({ loading: false, success: false, error: err });
      })
    );
  }
  public getPerformanceStatisticIntern(fd: string, td: string, branchId?: any): Observable<ApiResponse<SourceStatistic>> {
    branchId = branchId == null ? '' : branchId;
    return this.http.get<any>(this.rootUrl + `/GetPerformanceInternCVSource?fd=${fd}&td=${td}&branchId=${branchId}`).pipe(
      map(data => ({ ...data, loading: false })),
      startWith({ loading: true, success: false }),
      catchError((err: HttpErrorResponse) => {
        return of({ loading: false, success: false, error: err });
      })
    );
  }
  
  exportOverviewHiring(payload: {fromDate: string, toDate: string,userType: UserType, branchs?: any[] }): Observable<Blob> {
    return this.createExport(payload, '/ExportOverviewHiring');
  }
}
