import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { CreationTimeEnum } from '@shared/AppEnums';
import * as moment from 'moment';
import { DialogService } from 'primeng/dynamicdialog';
import { CREATION_TIME, DateFormat } from './../../AppConsts';
import { CustomDateDialogComponent } from './custom-date-dialog/custom-date-dialog.component';

type Action_Date = 'Previous' | 'Next';
export interface TalentDateTime {
  dateType: string,
  fromDate: moment.Moment,
  toDate: moment.Moment,
  dateText: string
}

@Component({
  selector: 'talent-date-selector',
  templateUrl: './date-selector.component.html',
  styleUrls: ['./date-selector.component.scss']
})

export class DateSelectorComponent implements OnInit {

  @Output() selectChange = new EventEmitter<TalentDateTime>();
  @Input() rowUI: boolean = false;
  @Input() defaultOption: string;
  @Input() label: string;
  @Input() timeOptions = CREATION_TIME;
  public readonly CreationTimeEnum = CreationTimeEnum;
  searchVal: FormControl = new FormControl();
  customizeView: number = 0;
  dateText: string;
  dateType: string = CreationTimeEnum.ALL;
  defaultLabel: string = 'Creation Time';
  isFistHalfYear: boolean = true;
  isBtnPrev: boolean;
  initOptionHalfYear: boolean = true;
  currentDate = moment();

  constructor(private _dialog: DialogService) { }

  ngOnInit(): void {
    this.setDefaultOption();
    this.setLabel();
  }

  setDefaultOption() {
    if (!this.defaultOption) return;
    this.searchVal.setValue(this.defaultOption);
    this.onSelectChange();
  }

  setLabel() {
    if (this.label) return;
    this.label = this.defaultLabel;
  }

  onPreOrNext(bahavior: Action_Date) {
    if (this.searchVal.value === CreationTimeEnum.CUSTOM || this.searchVal.value === '') {
      return;
    }
    if(bahavior === 'Previous'){
      this.customizeView--;
      this.isBtnPrev = true;
    }
    else{
      this.customizeView++
      this.isBtnPrev = false;
    }
    this.onSelectChange();
  }

  onSelectChange(reset?: boolean) {
    if (reset) {
      this.customizeView = 0;
       this.dateText = '';
    }

    const { unitOfTime, keyOfTime, typeDate } = this.getUnitOfTime(this.searchVal.value);
    this.dateType = typeDate;
    if (this.searchVal.value === CreationTimeEnum.HALF_YEAR) {
      this.selectHalfYear();
      return;
    }
    if (this.searchVal.value !== CreationTimeEnum.CUSTOM) {
      const fromDate = moment().clone().startOf(unitOfTime).add(this.customizeView, keyOfTime);
      const toDate = moment(fromDate).clone().endOf(unitOfTime);
      this.dateText = this.getDisplayText(typeDate, fromDate, toDate);
      this.selectChange.emit({ dateType: this.dateType, fromDate, toDate, dateText: this.dateText });
      return;
    }
    this.showPopup();
  }

  selectHalfYear() {
    let fromDate;
    let toDate ;
    if(this.initOptionHalfYear){
      fromDate = moment().startOf('year'); ;
      toDate = moment(fromDate).endOf('year').add(-6,'months');
      if(moment().quarter() > 2){
        fromDate = moment().startOf('year').add(6,'months'); ;
        toDate = moment(fromDate).endOf('year');
        this.isFistHalfYear = false;
      }
      this.initOptionHalfYear = false;
    }
    else{
      if(this.isFistHalfYear){
        if(this.isBtnPrev){
          this.currentDate = this.currentDate.add(-1,'years');
        }
        fromDate = this.currentDate.startOf('year').add(6,'months');
        toDate = moment(fromDate).endOf('year');
        this.isFistHalfYear = false;
      }
      else{
        if(this.isBtnPrev === false){
          this.currentDate = this.currentDate.add(1,'years');
        }
        fromDate = this.currentDate.startOf('year');
        toDate = moment(fromDate).endOf('year').add(-6,'months');
        this.isFistHalfYear = true;
      }
    }

    this.dateText = this.getDisplayText(CreationTimeEnum.HALF_YEAR, fromDate, toDate);
    this.selectChange.emit({ dateType: this.dateType, fromDate, toDate, dateText: this.dateText });
  }

  showPopup(): void {
    const dialogRef = this._dialog.open(CustomDateDialogComponent, {
      header: `Custom Date`,
      width: "30%",
      contentStyle: { "max-height": "500px", overflow: "visible" },
      baseZIndex: 10000,
      data: null,
    });

    dialogRef.onClose.subscribe((res: { from: moment.Moment, to: moment.Moment }) => {
      this.dateText = this.getDisplayText('Custom Time', res?.from, res?.to) || 'No date select'
      this.selectChange.emit({ dateType: this.dateType, fromDate: res?.from, toDate: res?.to, dateText: this.dateText });
    });
  }

  private getUnitOfTime(time: CreationTimeEnum): any {
    switch (time) {
      case CreationTimeEnum.DAY: {
        return {
          unitOfTime: CreationTimeEnum.DAY.toLowerCase(),
          keyOfTime: CreationTimeEnum.DAY.toLowerCase(),
          typeDate: CreationTimeEnum.DAY
        }
      }

      case CreationTimeEnum.WEEK: {
        return {
          unitOfTime: 'isoWeek',
          keyOfTime: 'w',
          typeDate: CreationTimeEnum.WEEK
        }
      }
      case CreationTimeEnum.MONTH: {
        return {
          unitOfTime: CreationTimeEnum.MONTH.toLowerCase(),
          keyOfTime: CreationTimeEnum.MONTH.toLowerCase(),
          typeDate: CreationTimeEnum.MONTH
        }
      }
      case CreationTimeEnum.Quarter: {
        return {
          unitOfTime: CreationTimeEnum.Quarter.toLowerCase(),
          keyOfTime: CreationTimeEnum.Quarter.toLowerCase(),
          typeDate: CreationTimeEnum.Quarter
        }
      }
      case CreationTimeEnum.HALF_YEAR: {
        return {
          unitOfTime: CreationTimeEnum.HALF_YEAR.toLowerCase(),
          keyOfTime: CreationTimeEnum.HALF_YEAR.toLowerCase(),
          typeDate: CreationTimeEnum.HALF_YEAR
        }
      }
      case CreationTimeEnum.YEAR: {
        return {
          unitOfTime: CreationTimeEnum.YEAR.toLowerCase(),
          keyOfTime: CreationTimeEnum.YEAR.toLowerCase(),
          typeDate: CreationTimeEnum.YEAR
        }

      }
      case CreationTimeEnum.CUSTOM: {
        return {
          unitOfTime: CreationTimeEnum.CUSTOM.toLowerCase(),
          keyOfTime: CreationTimeEnum.CUSTOM.toLowerCase(),
          typeDate: CreationTimeEnum.CUSTOM
        }
      }
      default: {
        return {
          unitOfTime: CreationTimeEnum.ALL.toLowerCase(),
          keyOfTime: CreationTimeEnum.ALL.toLowerCase(),
          typeDate: CreationTimeEnum.ALL
        }
      }
    }
  }

  private getDisplayText(typeDate: string, fromDate: moment.Moment = null, toDate: moment.Moment = null) {
    const spaceFormat = '\u00A0 \u2014 \u00A0';
    if (typeDate === CreationTimeEnum.ALL) {
      return '';
    }

    if (!fromDate || !toDate) {
      return '';
    }
    const toDateFormat = toDate.format(DateFormat.DD_MM_YYYY);
    switch (typeDate) {
      case CreationTimeEnum.DAY: {
        return toDateFormat;
      }
      case CreationTimeEnum.WEEK: {
        if(fromDate.format(DateFormat.MM) == toDate.format(DateFormat.MM)){
          return `${fromDate.format(DateFormat.DD)}${spaceFormat}${toDate.format(DateFormat.DD_MM_YYYY)}`
        }

        return `${fromDate.format(DateFormat.DD_MM)}${spaceFormat}${toDate.format(DateFormat.DD_MM_YYYY)}`
      }
      case CreationTimeEnum.MONTH: {
        return toDate.format(DateFormat.MM_YYYY);
      }
      case CreationTimeEnum.Quarter: {
        return `${fromDate.format(DateFormat.DD_MM)}${spaceFormat}${toDateFormat}`;
      }
      case CreationTimeEnum.HALF_YEAR: {
        return `${fromDate.format(DateFormat.DD_MM)}${spaceFormat}${toDate.format(DateFormat.DD_MM_YYYY)}`;
      }
      case CreationTimeEnum.YEAR: {
        return toDate.format(DateFormat.YYYY);
      }
      case CreationTimeEnum.CUSTOM: {
        return `${fromDate.format(DateFormat.DD_MM_YYYY)}${spaceFormat}${toDate.format(DateFormat.DD_MM_YYYY)}`
      }
      default: {
        return `${fromDate.format(DateFormat.DD_MM_YYYY)}${spaceFormat}${toDate.format(DateFormat.DD_MM_YYYY)}`
      }
    }
  }

}
