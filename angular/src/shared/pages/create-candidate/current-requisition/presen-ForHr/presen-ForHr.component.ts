import { Component, Injector, OnInit, Optional } from '@angular/core';
import {AppComponentBase} from '@shared/app-component-base';

import {DynamicDialogConfig, DynamicDialogRef} from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-PresenPresen-ForHr',
  templateUrl: './Presen-ForHr.component.html',
  styleUrls: ['./Presen-ForHr.component.scss']
})
export class PresenForHrComponent extends AppComponentBase implements OnInit {

  presenForHr:boolean = false;

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
      presenForHr: this.presenForHr 
    });
  }
}
