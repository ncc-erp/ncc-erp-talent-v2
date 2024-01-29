import { Injectable } from '@angular/core';
import {BehaviorSubject, Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DataSharingService {
  
  private guideLineSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  public guideLine$: Observable<any> = this.guideLineSubject.asObservable();

constructor() { }

updatePayload(capabilities: any) {
  this.guideLineSubject.next(capabilities);
}
}
