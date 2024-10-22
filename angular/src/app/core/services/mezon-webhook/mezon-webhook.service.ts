import { Injectable } from '@angular/core';
import { BaseApiService } from '../apis/base-api.service';
import { HttpClient} from '@angular/common/http';
import { ObservableInput, ObservedValueOf, OperatorFunction } from 'rxjs';

export declare function mergeMap<T, O extends ObservableInput<any>>(project: (value: T, index: number) => O, concurrent?: number): OperatorFunction<T, ObservedValueOf<O>>;

@Injectable({
  providedIn: 'root'
})
export class MezonWebhookService extends BaseApiService {

  constructor(
    public http: HttpClient
  ) {
    super(http);
  }

  changeUrl(): string {
    return 'MezonWebhook';
  }
  
}
