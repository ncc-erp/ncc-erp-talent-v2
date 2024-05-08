import {
  HttpClient,
  HttpErrorResponse,
} from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AppConsts } from "@shared/AppConsts";
import { Observable, of } from "rxjs";
import { catchError, map, startWith } from "rxjs/operators";
import { ICandidateReportExtractCV } from "@app/core/models/candidate/candidate.model";

@Injectable({
  providedIn: "root",
})
export class AutoBotApiService {
  get baseUrl() {
    return AppConsts.autoBotServiceBaseUrl.replace(/\/+$/, "");
  }

  constructor (public http: HttpClient) {
    this.http = http;
  }

  extractCV(file: FormData): Observable<ICandidateReportExtractCV> {
    return this.http
      .post<any>(this.baseUrl + "/extract-cv", file)
      .pipe(
        map(data => ({ ...data, loading: false })),
        startWith({ loading: true, success: false }),
        catchError((err: HttpErrorResponse) => {
          return of({ loading: false, success: false, error: err.error.error });
        })
      );
  }
}
