import { MailPreviewInfo } from './../../models/mail/mail.model';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError, map, startWith } from 'rxjs/operators';
import { BaseApiService } from '../apis/base-api.service';
import { ApiResponse } from './../../../../shared/paged-listing-component-base';
import { MAIL_TYPE } from '@shared/AppEnums';

@Injectable({
    providedIn: 'root'
})
export class MailService extends BaseApiService {

    constructor(
        public http: HttpClient
    ) {
        super(http);
    }

    changeUrl(): string {
        return 'Mail';
    }

    getByIdFakeData(id): Observable<ApiResponse<MailPreviewInfo>> {
        return this.get("/GetByIdFakeData?id=" + id);
    }

    getPreviewMail(emailType: MAIL_TYPE, requestCvId: number): Observable<ApiResponse<MailPreviewInfo>> {
        return this.get(`/PreviewBeforeSend?id=${requestCvId}&emailType=${emailType}`);
    }

    sendMail(data: MailPreviewInfo): Observable<ApiResponse<string>> {
        return this.create(data, "/SendMail")
    }
}
