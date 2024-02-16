import { Injectable } from '@angular/core';
import {BaseApiService} from '../apis/base-api.service';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class CandidateLanguageService extends BaseApiService{

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }
  
  changeUrl(): string {
    return 'CandidateLanguage';
   }
  }