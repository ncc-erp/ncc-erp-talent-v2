import { ApiResponse } from './../../../../shared/paged-listing-component-base';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from '../apis/base-api.service';

@Injectable({
  providedIn: 'root'
})
export class ExportFakeService extends BaseApiService {

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'ExportFakeForSaleService';
  }

  ExportCVFake(data: object): Observable<ApiResponse<any>> {
    return this.create(data, '/ExportCVFake', );
  }
}
