import { ApiResponse } from '@shared/paged-listing-component-base';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from '../apis/base-api.service';
import { Observable } from 'rxjs';
import { ProfileVersion } from '@app/core/models/employee-profile/profile-model';

@Injectable({
  providedIn: 'root'
})
export class VersionService extends BaseApiService {

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'Version';
  }

  getAllVersion(employeeId: number): Observable<ApiResponse<any>> {
    return this.get(`/GetAllVersion?employeeId=${employeeId}`);
  }

  createVersion(payload: ProfileVersion): Observable<ApiResponse<any>> {
    return this.create(payload, '/CreateVersion');
  }

  deleteVersion(id: number): Observable<ApiResponse<any>> {
    return this.delete(id, '/DeleteVersion?id=' + id);
  }
}
