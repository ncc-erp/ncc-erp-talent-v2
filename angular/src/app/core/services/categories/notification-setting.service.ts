import { Injectable } from '@angular/core';
import { BaseApiService } from '../apis/base-api.service';
import { HttpClient} from '@angular/common/http';
import { Observable, ObservableInput, ObservedValueOf, OperatorFunction } from 'rxjs';
import { MessageTemplateDto } from '@app/core/models/categories/notification-setting.model';
import { ApiResponse } from '@shared/paged-listing-component-base';

export declare function mergeMap<T, O extends ObservableInput<any>>(project: (value: T, index: number) => O, concurrent?: number): OperatorFunction<T, ObservedValueOf<O>>;

@Injectable({
  providedIn: 'root'
})
export class NotificationSettingService extends BaseApiService {

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'NotificationSetting';
  }

  getMessageTemplateById(id: number): Observable<ApiResponse<MessageTemplateDto>> {
    return this.http.get<any>(this.rootUrl + "/GetMessageTemplate?notificationId=" + id);
  }

  getFakeData(id: number): Observable<ApiResponse<MessageTemplateDto>> {
    return this.http.get<any>(this.rootUrl + "/GetFakeDataById?notificationId=" + id);
  }
}