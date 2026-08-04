import { Component, Injector, Input, OnInit } from '@angular/core';
import { UtilitiesService } from '@app/core/services/utilities.service';
import { ReportInternService } from '@app/core/services/report/report-intern.service';
import { ToastMessageType } from '@shared/AppEnums';
import { AppComponentBase } from '@shared/app-component-base';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { finalize } from 'rxjs/operators';

interface ExportStatusOption {
  id: number;
  name: string;
}

interface ExportBranch {
  id: string | number;
  displayName: string;
}

@Component({
  selector: 'talent-export-intern-education',
  templateUrl: './export-intern-education.component.html',
  styleUrls: ['./export-intern-education.component.scss']
})
export class ExportInternEducationComponent extends AppComponentBase implements OnInit {
  @Input() fromDate: string;
  @Input() toDate: string;
  @Input() branchs: ExportBranch[] = [];

  loading = false;
  selectedCVStatus: number | null = null;
  selectedCandidateStatus: number | null = null;
  candidateStatusOptions: ExportStatusOption[] = [];

  readonly cvStatusOptions: ExportStatusOption[] = [
    { id: 0, name: 'New - Unprocessed' },
    { id: 1, name: 'New - Normal' },
    { id: 2, name: 'Contacting' },
    { id: 3, name: 'Passed' },
    { id: 4, name: 'Failed' },
    { id: 5, name: 'Draft' }
  ];

  constructor(
    injector: Injector,
    public bsModalRef: BsModalRef,
    private _reportIntern: ReportInternService,
    private _utilities: UtilitiesService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.candidateStatusOptions = (this._utilities.catReqCvStatus || [])
      .map(status => ({ id: status.id, name: status.name }));
  }

  closePopup(): void {
    this.bsModalRef.hide();
  }

  private isValidDate(value: string): boolean {
    if (!/^\d{4}-\d{2}-\d{2}$/.test(value || '')) {
      return false;
    }

    const [year, month, day] = value.split('-').map(Number);
    const parsedDate = new Date(year, month - 1, day);
    return parsedDate.getFullYear() === year
      && parsedDate.getMonth() === month - 1
      && parsedDate.getDate() === day;
  }

  onExport(): void {
    if (!this.isValidDate(this.fromDate) || !this.isValidDate(this.toDate)) {
      this.showToastMessage(ToastMessageType.ERROR, 'Full date required');
      return;
    }

    if (this.fromDate > this.toDate) {
      this.showToastMessage(ToastMessageType.ERROR, 'From Date must not be greater than To Date');
      return;
    }

    if (this.selectedCVStatus === null && this.selectedCandidateStatus === null) {
      this.showToastMessage(
        ToastMessageType.ERROR,
        'Please select at least one CV Status or Candidate Status'
      );
      return;
    }

    this.loading = true;
    const payload = {
      fromDate: this.fromDate,
      toDate: this.toDate,
      branchs: this.branchs,
      cvStatuses: this.selectedCVStatus === null ? [] : [this.selectedCVStatus],
      candidateStatuses: this.selectedCandidateStatus === null ? [] : [this.selectedCandidateStatus]
    };

    this.subs.add(
      this._reportIntern.exportInternEducation(payload)
        .pipe(finalize(() => this.loading = false))
        .subscribe((result: Blob) => {
          if (result.size > 0) {
            const blob = new Blob([result], { type: 'application/octet-stream' });
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `InternEducation_${this.fromDate} - ${this.toDate}.xlsx`;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
          }

          this.bsModalRef.hide();
        })
    );
  }
}
