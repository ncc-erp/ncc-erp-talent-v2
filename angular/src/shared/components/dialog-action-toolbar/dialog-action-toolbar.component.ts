import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ActionEnum } from '@shared/AppEnums';
import { DynamicDialogRef } from 'primeng/dynamicdialog';

@Component({
  selector: 'talent-dialog-action-toolbar',
  templateUrl: './dialog-action-toolbar.component.html',
  styleUrls: ['./dialog-action-toolbar.component.scss']
})
export class DialogActionToolbarComponent implements OnInit {

  @Input() dialogRef: DynamicDialogRef;
  @Input() disabledSaveBtn: boolean;
  @Input() showButtonCancel = true;
  @Input() showButtonSaveClose = false;
  @Input() showButtonSave = true;
  @Input() showButtonSaveCloseActionCreate : string;
  @Output() onSave = new EventEmitter<boolean>(false);

  public isSave = false;
  public actionCreate = ActionEnum.CREATE;

  constructor() { }

  ngOnInit(): void {
    this.isSave = !!this.onSave.observers.length;
  }

  save(isClose = false) {
    if(this.disabledSaveBtn) return;
    this.onSave.emit(isClose);
  }

}
