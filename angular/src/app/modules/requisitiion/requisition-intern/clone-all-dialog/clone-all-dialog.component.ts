import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/app-component-base';
import { DateFormat, MESSAGE } from '@shared/AppConsts';
import { SortType, ToastMessageType } from '@shared/AppEnums';
import * as moment from 'moment';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CloseCloneAllRequestInternPayload, RequisitionIntern } from './../../../../core/models/requisition/requisition.model';
import { RequisitionInternService } from './../../../../core/services/requisition/requisition-intern.service';
import { UtilitiesService } from './../../../../core/services/utilities.service';

@Component({
  selector: 'talent-clone-all-dialog',
  templateUrl: './clone-all-dialog.component.html',
  styleUrls: ['./clone-all-dialog.component.scss']
})
export class CloneAllDialogComponent extends AppComponentBase implements OnInit {

  readonly DATE_FORMAT = DateFormat;
  readonly SORT_TYPE = SortType;

  reqInterns: RequisitionIntern[] = [];
  editingRows: { [s: string]: boolean } = {};

  constructor(
    injector: Injector,
    public _utilities: UtilitiesService,
    private _reqIntern: RequisitionInternService,
    public ref: DynamicDialogRef,
    public config: DynamicDialogConfig
  ) {
    super(injector);
  }

  ngOnInit(): void { 
    this.getAllCloseAndCloneRequisition();
  }

  getAllCloseAndCloneRequisition() {
    this.subs.add(
      this._reqIntern.getAllCloseAndCloneRequisition().subscribe((rs) => 
      {
        this.reqInterns = [];
        this.isLoading = rs.loading;
        if (rs.success) {
          this.reqInterns = rs.result;
          this.reqInterns.forEach(item => {
            this.editingRows[item.id] = true;
            item.quantity = 0;
            item.note = '';

            const date = item.timeNeed ? new Date(item.timeNeed) : new Date();
            if (date) date.setMonth(date.getMonth() + 1);
            item.timeNeed = date;
          })
        }
      }))
  }



  onSave(isClose: boolean) {
    const payload: CloseCloneAllRequestInternPayload = {
      listRequisitionIntern: this.reqInterns.map(item => {
        return {
          requestId: item.id,
          quantity: item.quantity,
          note: item.note,
          timeNeed: moment(item.timeNeed).format(DateFormat.YYYY_MM_DD),
        }
      })
    }

    this.subs.add(
      this._reqIntern.closeAndCloneAllRequisition(payload).subscribe(res => {
        this.isLoading = res.loading;
        if (res.success) {
          this.showToastMessage(ToastMessageType.SUCCESS, MESSAGE.CLONE_SUCCESS);
          this.ref.close(res.success);
        }
      })
    );
    return;
  }
}
