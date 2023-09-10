import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AppComponentBase } from '@shared/app-component-base';
import * as moment from 'moment';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { CustomValidators } from './../../../../app/core/helpers/validator.helper';

@Component({
  selector: 'talent-custom-date-dialog',
  templateUrl: './custom-date-dialog.component.html',
  styleUrls: ['./custom-date-dialog.component.scss']
})
export class CustomDateDialogComponent extends AppComponentBase implements OnInit {

  submitted: boolean = false;
  form: FormGroup;

  constructor(
    public ref: DynamicDialogRef,
    public dialogService: DialogService,
    private injector: Injector,
    private _fb: FormBuilder,
  ) {
    super(injector);
  }

  get formControls() {
    return this.form.controls;
  }

  ngOnInit(): void {
    this.initForm();
  }

  onSave() {
    this.submitted = true;
    if (this.form.invalid) return;

    const date = {
      from: moment(this.formControls['fromDate'].value),
      to: moment(this.formControls['toDate'].value),
    }
    this.closePopup(date)
  }

  closePopup(date: { from: moment.Moment, to: moment.Moment }) {
    this.submitted = false;
    this.ref.close(date);
  }

  private initForm() {
    this.form = this._fb.group({
      fromDate: [new Date(), [Validators.required]],
      toDate: [new Date(), [Validators.required]],
    }, {
      validator: [
        CustomValidators.isToDateMustGreaterThanOrEqual('fromDate', 'toDate')
      ]
    });
  }
}
