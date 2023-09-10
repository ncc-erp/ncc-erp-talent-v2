import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CapabilitySetting, CapabilitySettingClone, CapabilitySettingCreateResponse } from '@app/core/models/categories/capabilities-setting.model';
import { Observable, of } from 'rxjs';
import { catchError, map, startWith } from 'rxjs/operators';
import { BaseApiService } from '../apis/base-api.service';
import { ApiResponse } from './../../../../shared/paged-listing-component-base';

@Injectable({
  providedIn: 'root'
})
export class CapabilitySettingService extends BaseApiService {

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'CapabilitySetting';
  }

  getUserTypes(): Observable<ApiResponse<any>> {
    return this.http
      .get<any>(this.rootUrl + "/GetUserType")
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err });
        })
      );
  }

  getAllCapabilitiesSettings(params?: { capabilityName: string, userType: number, subPositionId: number , factor: number, fromType: boolean}): Observable<ApiResponse<CapabilitySetting[]>> {
    return this.http
      .get<any>(this.rootUrl + "/GetAllCapabilitySettings", {
        params: new HttpParams().appendAll({
          capabilityName: params.capabilityName ?? '',
          userType: params.userType ?? '',
          subPositionId: params.subPositionId ?? '',
            factor: params.factor ?? '',
            fromType: params.fromType ?? ''
        })
      })
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err });
        })
      );
  }

  getCapabilitiesByUserTypeAndPositionId(params: { userType: number, subPositionId: number }): Observable<ApiResponse<any>> {
    return this.http
      .get<any>(this.rootUrl + "/getCapabilitiesByUserTypeAndPositionId", {
        params: new HttpParams().appendAll({
          userType: params.userType ?? '',
          subPositionId: params.subPositionId ?? ''
        })
      })
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err });
        })
      );
  }

  GetRemainCapabilitiesByUserTypeAndPositionId(params: { userType: number, subPositionId: number }): Observable<ApiResponse<any>> {
    return this.http
      .get<any>(this.rootUrl + "/GetRemainCapabilitiesByUserTypeAndPositionId", {
        params: new HttpParams().appendAll({
          userType: params.userType ?? '',
          subPositionId: params.subPositionId ?? ''
        })
      })
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err });
        })
      );
  }

  deleteCapabilitySetting(userTypeId: number, subPositionId: number): Observable<ApiResponse<any>> {
    return this.http
      .delete<any>(this.rootUrl + "/DeleteGroupCapabiliSettings", {
        params: new HttpParams()
          .set("userType", userTypeId)
          .set("subPositionId", subPositionId)
      }).pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err });
        })
      );
  }

  capabilitySettingClone(payload: CapabilitySettingClone): Observable<ApiResponse<CapabilitySettingClone>> {
    return this.create(payload, '/CapabilitySettingClone');
  }
  updateFactor(payload: CapabilitySettingCreateResponse[]): Observable<ApiResponse<any>>{
    return this.update(payload, '/UpdateFactor');
  }
}
