import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SkillCandidateDto } from '@app/core/models/employee-profile/profile-model';
import { Observable } from 'rxjs';
import { BaseApiService } from '../apis/base-api.service';
import { ApiResponse } from './../../../../shared/paged-listing-component-base';
import { CatalogModel } from './../../models/common/common.dto';

@Injectable({
  providedIn: 'root'
})
export class CommonEmployeeService extends BaseApiService {

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'CommonEmployee';
  }

  getPosition(): Observable<ApiResponse<CatalogModel[]>> {
    return this.getAll("/GetPosition");
  }

  getLanguage(): Observable<ApiResponse<CatalogModel[]>> {
    return this.getAll("/GetLanguage");
  }

  getCBBGroupSkill(): Observable<ApiResponse<CatalogModel[]>> {
    return this.getAll("/GetCBBGroupSkill");
  }

  getSkillByGroupSkillId(groupSkillId: number): Observable<ApiResponse<SkillCandidateDto[]>> {
    return this.get(`/GetCBBSkillByGroupSkillId?id=${groupSkillId}`);
  }


}
