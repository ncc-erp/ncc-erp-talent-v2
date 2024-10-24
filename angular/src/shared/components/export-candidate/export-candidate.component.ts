import { Component, Injector, Input } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CandidateInternService } from '@app/core/services/candidate/candidate-intern.service';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastMessageType, UserType } from '@shared/AppEnums';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { CandidateReportPayload } from '@app/core/models/candidate/candidate.model';
import { finalize } from 'rxjs/operators';
@Component({
  selector: 'talent-export-candidate',
  templateUrl: './export-candidate.component.html',
  styleUrls: ['./export-candidate.component.scss']

})
export class ExportCandidateComponent extends AppComponentBase {
  fromDate: string ='' ;
  toDate: string ='' ;
  exportForm: FormGroup;
  loading = false;
  infomation:boolean = false ;
  reqCvStatus: number;
  reqCvToStatus: number;
  reqCvFromStatus: number;
  statusHistory: boolean = false;

  @Input() userType: UserType.INTERN | UserType.STAFF;

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    public _candidateIntern: CandidateInternService,
    public _utilities: UtilitiesService,
    private _fb: FormBuilder,
      ) {
    super(injector);
    const currentDate = new Date();
    this.fromDate = currentDate.toISOString().substr(0, 7);
    this.toDate = currentDate.toISOString().substr(0, 7);
  }

  formatDate(dateObj) {
  const year = dateObj.getFullYear();
  const month = String(dateObj.getMonth() + 1).padStart(2, '0');
  const day = String(dateObj.getDate()).padStart(2, '0');
  return `${year}/${month}/${day}`;
  }

  closePopup(): void {
    this.bsModalRef.hide();
  }
  onDropdownChange(status: any) {
    this.reqCvStatus = status.value;
  }
  onDropdownFromStatus(fromstatus: any) {
    this.reqCvFromStatus = fromstatus.value;
  }
  onDropdownToStatus(tostatus: any) {
    this.reqCvToStatus = tostatus.value;
  }

  public onExportReport() {
    this.loading = true;
    const userType = this.userType === UserType.INTERN ? 0 : 1;
    if (!this.fromDate || !this.toDate) {
      this.showToastMessage(ToastMessageType.ERROR,'Full date required');
      this.loading = false;
      return;
    }

     const fromDateParts = this.fromDate.split('-');
     const fromDateYear = Number(fromDateParts[0]);
     const fromDateMonth = Number(fromDateParts[1]);
     const fromDateDay = 1;
     const startDate = this.formatDate(new Date(fromDateYear, fromDateMonth - 1, fromDateDay)) ;
     const formattedFromDate = startDate.split('T')[0];

     const toDateParts = this.toDate.split('-');
     const toDateYear = Number(toDateParts[0]);
     const toDateMonth = Number(toDateParts[1]);
     const toDateDay = new Date(toDateYear, toDateMonth, 0).getDate();
     const endDate = this.formatDate(new Date(toDateYear, toDateMonth - 1, toDateDay)) ;
     const formattedToDate = endDate.split('T')[0];

    if (fromDateYear > toDateYear) {
      this.showToastMessage(ToastMessageType.ERROR,'Year from date cannot be greater than year to date.');
      this.loading = false;
      return;
    }

    if (fromDateYear === toDateYear && parseInt(fromDateParts[1]) > parseInt(toDateParts[1])) {
      this.showToastMessage(ToastMessageType.ERROR,'Month from date must not be greater than month to day in the same year.');
      this.loading = false;
      return;
    }

      const fileName = userType === 0 ? 'InternReport_'+ this.fromDate + '_' +this.toDate +'.xlsx' : 'StaffReport_'+ this.fromDate + '_' +this.toDate +'.xlsx';
      const payload: CandidateReportPayload = {
      userType: userType ,
      fromDate: formattedFromDate,
      toDate: formattedToDate,
      reqCvStatus: this?.reqCvStatus,
      toStatus: this.statusHistory ? this.reqCvToStatus : null,
      fromStatus: this.statusHistory ? this.reqCvFromStatus : null,
    };
    this.subs.add(
      this._candidateIntern.exportReport(payload).pipe(finalize(() => this.loading = false)).subscribe(
       (result: Blob) => {
        if (result.size > 0) {
          const blob = new Blob([result], { type: 'application/octet-stream' });
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          a.download = fileName;
          document.body.appendChild(a);
          a.click();
          document.body.removeChild(a);
          window.URL.revokeObjectURL(url);
        }
      },
    ));
  }

}
