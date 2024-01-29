import { Component, Injector, OnInit, Optional } from '@angular/core';
import {AppComponentBase} from '@shared/app-component-base';

import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-confirm-presentforhr',
  templateUrl: './confirm-presentforhr.component.html',
  styleUrls: ['./confirm-presentforhr.component.scss']
})
export class ConfirmPresentForHr extends AppComponentBase implements OnInit {

  isPresentForHr:boolean = false;

  constructor(
    injector: Injector,
    public config: DynamicDialogConfig,
    public dialogRef: DynamicDialogRef,
  ) {
    super(injector);
   }

  ngOnInit() {
  }
  Save(){
    this.dialogRef.close({
      isPresentForHr: this.isPresentForHr 
    });
  }
}
