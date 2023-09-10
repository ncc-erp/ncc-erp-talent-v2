import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from '../apis/base-api.service';
import { ExternalCv } from '@app/core/models/categories/external-cv.model';
import { ApiResponse } from './../../../../shared/paged-listing-component-base';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExternalCvService extends BaseApiService{

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }
  
  changeUrl(): string {
   return 'ExternalCV';
  }
  getExternalCvById(id: number): Observable<ApiResponse<ExternalCv>> {
    return this.get('/GetExternalCVById?cvId=' + id);
  }
}