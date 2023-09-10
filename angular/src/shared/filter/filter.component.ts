import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import * as moment from 'moment';

@Component({
  selector: 'app-filter',
  templateUrl: './filter.component.html',
  styleUrls: ['./filter.component.scss']
})

export class FilterComponent {
  @Input() inputFilters: InputFilterDto[];
  @Input() item: any;
  @Output() emitChange = new EventEmitter<any>();
  @Output() deleteDataFilter = new EventEmitter<any>();
  value: any;
  dropdownData: any[] = []
  filterType: number = 0;
  search: any = "";
  public FilterType = FilterType;
  comparisions: ComparisionDto[] = [];
  constructor() {
  }
  ngOnInit(): void {
    if (this.item.propertyName !== '') {
      let comps = this.inputFilters.find(i => i.propertyName === this.item.propertyName)?.comparisions || [0];
      comps.forEach(element => {
        this.comparisions.push({ id: element, name: COMPARISIONS[element] } as ComparisionDto);
      });
    }
    this.filterType = this.item.filterType
    this.dropdownData = this.item.dropdownData

  }
  onChange(value: string | number, name: string): void {
    if (name === 'propertyName') {
      this.item.value = ''
      this.emitChange.emit({ name: 'comparision', value: undefined })
      if (value == '') {
        this.comparisions = [];
        return;
      }
      var comps = this.inputFilters.find(i => i.propertyName === value).comparisions;
      this.comparisions = [];
      comps.forEach(element => {
        this.comparisions.push({ id: element, name: COMPARISIONS[element] } as ComparisionDto);
      });
      this.inputFilters.forEach(item => {
        if (item.propertyName == value) {
          this.filterType = item.filterType
          this.item.filterType = item.filterType
          switch (this.filterType) {
            case FilterType.DatePicker: this.item.value = moment(new Date()).format("YYYY-MM-DD")
              break;
            case FilterType.RadioYesNo:
            case FilterType.RadioDoneNotDone:
              this.item.value = true
              break;
            case FilterType.Dropdown:
            case FilterType.DropdownWithSearch:
              this.dropdownData = item.dropdownData, this.item.dropdownData = item.dropdownData
              break;
          }
        }
        return;
      })
    }
    this.emitChange.emit({ name, value })

  }
  onDateChange() {
    this.item.value = moment(this.item.value).format("YYYY-MM-DD")
  }
  onRadioChange(event) {
    this.item.value = event.value
  }
  onDropdownChange(data) {
    this.item.value = data
  }
  deleteFilter() {
    this.deleteDataFilter.emit();
  }
}

export class InputFilterDto {
  propertyName: string;
  displayName: string;
  comparisions: number[];
  filterType?: number;
  dropdownData?: DropDownDataDto[]
}

export class ComparisionDto {
  id: number;
  name: string;
}


export const COMPARISIONS: string[] =
  ['Bằng',
    'Nhỏ hơn',
    'Nhỏ hơn hoặc bằng',
    'Lớn hơn',
    'Lớn hơn hoặc bằng',
    'Không bằng',
    'Chứa kí tự',
    'Bắt đầu với',
    'Kết thúc bằng',
    'Trong']
export class DropDownDataDto {
  value: any;
  displayName: any
}

export enum FilterType {
  Text = 0,
  DatePicker = 1,
  RadioYesNo = 2,
  Dropdown = 3,
  RadioDoneNotDone = 4,
  DropdownWithSearch = 5
}