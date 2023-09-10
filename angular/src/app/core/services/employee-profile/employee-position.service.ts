import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseApiService } from '../apis/base-api.service';


@Injectable({
  providedIn: 'root'
})
export class EmployeePositionService extends BaseApiService {

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
   return 'EmployeePosition';
  }
}
