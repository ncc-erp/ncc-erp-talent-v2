import { ApiResponse } from '../../../../shared/paged-listing-component-base';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from '../apis/base-api.service';

@Injectable({
  providedIn: 'root'
})
export class ExportService extends BaseApiService {

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'ExportDocService';
  }

  exportCV(userId: number, typeOffile: number, isHiddenYear: boolean, versionId: number): Observable<any> {
    return this.get(`/ExportCV?userId=${userId}&typeOffile=${typeOffile}&isHiddenYear=${isHiddenYear}&versionId=${versionId}`);
  }
}
