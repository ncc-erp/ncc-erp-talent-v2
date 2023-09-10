import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from '../apis/base-api.service';
import { UserType } from './../../../../shared/AppEnums';
import { ApiResponse } from './../../../../shared/paged-listing-component-base';
import { nullToEmpty } from './../../../core/helpers/utils.helper';
import { EducationStatistic } from './../../models/report/report-education.model';

@Injectable({
  providedIn: 'root'
})
export class ReportInternService extends BaseApiService {

  changeUrl(): string {
    return 'Report';
  }

  params = (fd: string, td: string, branchId: number) => {
    return new HttpParams()
      .set('fd', nullToEmpty(fd))
      .set('td', nullToEmpty(td))
      .set('userType', UserType.INTERN)
      .set('branchId', nullToEmpty(branchId));
  }

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  public getEducationPassTest(fd: string, td: string, branchId: number): Observable<ApiResponse<EducationStatistic>> {
    return this.get(`/GetEducationPassTest?${this.params(fd, td, branchId).toString()}`);
  }

  public getEducationInternOnboarded(fd: string, td: string, branchId: number): Observable<ApiResponse<EducationStatistic>> {
    return this.get(`/GetEducationInternOnboarded?${this.params(fd, td, branchId).toString()}`);
  }
}
