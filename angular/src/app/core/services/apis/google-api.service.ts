import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { BaseApiService } from './base-api.service';

@Injectable({
  providedIn: 'root'
})
export class GoogleLoginService extends BaseApiService {

  constructor(
    http: HttpClient
  ) {
    super(http);
  }
  changeUrl() {
    return 'Task';
  }
  googleAuthenticate(googleToken: string): Observable<any> {
    return this.http.post(this.baseUrl + '/api/TokenAuth/GoogleAuthenticate', {googleToken: googleToken, secretCode: ''});
  }
}
