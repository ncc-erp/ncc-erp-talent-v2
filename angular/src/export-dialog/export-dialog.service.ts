import { Injectable } from '@angular/core';
import { BaseApiService } from '../app/core/services/apis/base-api.service';
import {HttpClient} from '@angular/common/http';
import {ReportOverviewService } from '../app/core/services/report/report-overview.service';
import {ReportInternService } from '../app/core/services/report/report-intern.service';
import {BehaviorSubject, Observable, Subject} from 'rxjs';
import {UserType} from '@shared/AppEnums';
import {takeUntil} from 'rxjs/operators';
interface sendataIntern {
  fromDate: string,
  toDate: string,
  branchs: {
    id: string | number;
    displayName: string;
  }[],
}
interface sendataOverview {
  fromDate: string,
  toDate: string,
  userType: UserType
  branchs: {
    id: string | number;
    displayName: string;
  }[],
}
@Injectable({
  providedIn: 'root'
})
export class ExportDialogService extends BaseApiService {
  private cancelExport$ = new Subject<void>(); 
  private downloadStatusSubject = new BehaviorSubject<boolean>(false);
constructor(
  public http: HttpClient,
  private _report: ReportOverviewService,
  private _reportIntern: ReportInternService,
) {
  super(http);
}

get downloadStatus$(): Observable<boolean> {
  return this.downloadStatusSubject.asObservable();
}

exportInternEducation(payload: sendataIntern): void {
  this.downloadStatusSubject.next(true);
  this._reportIntern.exportInternEducation(payload )
  .pipe(
    takeUntil(this.cancelExport$) 
  )
  .subscribe(
      (result: Blob) => {
      if (result.size > 0) {
        const blob = new Blob([result], { type: 'application/octet-stream' });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = 'InternEducation_'+ payload.fromDate +' - '+ payload.toDate +'.xlsx';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      }
      this.downloadStatusSubject.next(false);
    },
  );
}

exportOverviewHiring(payload: sendataOverview){
  this.downloadStatusSubject.next(true);
  this._report.exportOverviewHiring(payload )
  .pipe(
    takeUntil(this.cancelExport$) 
  )
  .subscribe(
    (result: Blob) => {
    if (result.size > 0) {
      const blob = new Blob([result], { type: 'application/octet-stream' });
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      if(payload.userType === 0){ 
        a.download = 'OverviewHiringIntern_'+ payload.fromDate + ' - ' + payload.toDate +'.xlsx'
        } else
        if(payload.userType === 1){
          a.download = 'OverviewHiringStaff_'+ payload.fromDate  + ' - ' + payload.toDate +'.xlsx'
        }
        else{
          a.download = 'OverviewHiringAll_'+ payload.fromDate  + ' - ' + payload.toDate +'.xlsx'
        }
      document.body.appendChild(a);
      a.click();
      document.body.removeChild(a);
      window.URL.revokeObjectURL(url);
    }
    this.downloadStatusSubject.next(false);
  },
);
}

cancelExport() {
  this.cancelExport$.next();
}
getDownloadStatus(): Observable<boolean> {
  return this.downloadStatusSubject.asObservable();
}

changeUrl(): string {
  throw new Error('Method not implemented.');
}
}
