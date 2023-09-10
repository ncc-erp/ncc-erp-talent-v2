import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from '../apis/base-api.service';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { LmsCourse } from '@app/core/models/categories/position-setting.model';

@Injectable({
  providedIn: 'root'
})
export class PositionSettingService extends BaseApiService{

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }
  
  changeUrl(): string {
   return 'PositionSetting';
  }

  getListCourse(): Observable<ApiResponse<LmsCourse[]>> {
    return this.getAll("/GetListCourse");
  }
}
