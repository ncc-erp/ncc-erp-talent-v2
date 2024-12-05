import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, } from '@angular/common/http';
import { BaseApiService } from '../apis/base-api.service';
import { of, Observable} from 'rxjs';
import { ApiResponse} from '@shared/paged-listing-component-base';
import { ICandidateReportExtractCV } from "@app/core/models/candidate/candidate.model";
import { catchError, map, startWith } from "rxjs/operators";

@Injectable({
    providedIn: 'root'
})
  
export class AutobotService extends BaseApiService {
    constructor(public http: HttpClient) {
        super(http);
    }
    
    changeUrl(): string {
        return "Autobot";
    }

    extractCV(file: FormData): Observable<ApiResponse<ICandidateReportExtractCV>> {
        return this.http
            .post<any>(this.rootUrl + "/GetCVExtractionData", file)
            .pipe(
            map(data => ({ ...data, loading: false })),
            startWith({ loading: true, success: false }),
            catchError((err: HttpErrorResponse) => {
                return of({ loading: false, success: false, error: err.error.error });
            })
        );
    }
}
