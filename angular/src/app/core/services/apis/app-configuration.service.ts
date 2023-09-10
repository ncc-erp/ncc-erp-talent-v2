import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseApiService } from './base-api.service';

@Injectable({
  providedIn: 'root'
})
export class AppConfigurationService extends BaseApiService {

  changeUrl() {
    return 'Configuration';
  }

  constructor( http: HttpClient) { 
    super(http)
  }
  getConfiguration():Observable<any>{
    return this.http.get(this.rootUrl + '/Get')
  }
  getGoogleClientAppId():Observable<any>{
    return this.http.get(this.rootUrl + '/GetGoogleClientAppId')
  }
  editConfiguration(item:any):Observable<any>{
    return this.http.post(this.rootUrl + '/Change', item)
  }
}
