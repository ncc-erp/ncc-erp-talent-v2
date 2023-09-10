import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError, map, startWith } from 'rxjs/operators';
import { BaseApiService } from '../apis/base-api.service';
import { ApiResponse } from './../../../../shared/paged-listing-component-base';
import { FormDialogCreateScoreSetting, FormDialogUpdateScoreRange, ParamsGetScoreRange, ScoreRangeWithSetting, ScoreSetting } from '@app/core/models/categories/score-range-setting.model';

@Injectable({
  providedIn: 'root'
})

export class ScoreSettingService extends BaseApiService {

  constructor(public http: HttpClient) {
    super(http);
  }

  changeUrl(): string {
    return 'ScoreSetting';
  }

  getAllRange(params?: ParamsGetScoreRange): Observable<ApiResponse<ScoreRangeWithSetting[]>> {
    return this.http
      .get<any>(this.rootUrl + "/GetAllRange", {
        params: new HttpParams().appendAll({
          UserType: params.userType ?? '',
          SubPositionId: params.subPositionId ?? '',
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

  createScoreSetting(payload: FormDialogCreateScoreSetting): Observable<ApiResponse<ScoreSetting[]>> {
    return this.create(payload);
  }

  updateScoreRange(payload: FormDialogUpdateScoreRange): Observable<ApiResponse<ScoreSetting[]>> {
    return this.update(payload);
  }

  deleteScoreRange(scoreRangeId: number): Observable<ApiResponse<string>> {
    return this.delete(scoreRangeId);
  }
}
