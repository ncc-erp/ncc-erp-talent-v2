import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CatalogModel } from '@app/core/models/common/common.dto';
import { ApiResponse } from '@shared/paged-listing-component-base';
import { Observable } from 'rxjs';
import { BaseApiService } from '../apis/base-api.service';

@Injectable({
  providedIn: 'root'
})
export class PostService extends BaseApiService{

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }
  
  changeUrl(): string {
   return 'Post';
  }

  getAllRecruitmentUser(): Observable<ApiResponse<CatalogModel[]>> {
    return this.getAll("/GetAllRecruitmentUser");
  }
}