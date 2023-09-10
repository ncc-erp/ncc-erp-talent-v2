import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { SearchType } from '@shared/AppEnums';
import { MultiSelect } from 'primeng/multiselect';

export interface HeaderSelectionConfig {
  labelName: string;
  defaultValue: string;
  selectOptions: {label: string, value: string}[]
}

export class SelectOptionConfig {
  labelName: string;
  catalogList: [];
  optionLabel?: string;
  optionValue?: any;
  userName?: string;
}

@Component({
  selector: 'talent-multi-select-h-radio',
  templateUrl: './multi-select-h-radio.component.html',
  styleUrls: ['./multi-select-h-radio.component.scss']
})

export class MultiSelectHRadioComponent implements OnInit {
  public readonly SearchType = SearchType;
  @ViewChild('multiSelectRadio') multiSelectRadio: MultiSelect;
  @Input() appendTo: string;
  @Input() valueSeletecd: any[];
  @Input() catalogConfig: SelectOptionConfig;
  @Input() headerSelectionConfig: HeaderSelectionConfig = {
    labelName: 'Search Type',
    defaultValue: SearchType.OR,
    selectOptions: [
      { label: SearchType.OR.toUpperCase(), value: SearchType.OR },
      { label: SearchType.AND.toUpperCase(), value: SearchType.AND }
    ]
  }

  @Output() onChange = new EventEmitter<any>();
  @Output() onSearchTypeChange = new EventEmitter<string>();

  searchWithCatalog: any[];
  searchType: string;

  constructor(
  ) { 
    this.searchType = this.headerSelectionConfig?.defaultValue;
  }

  ngOnInit(): void {
  }

  ngOnChanges() {
    this.searchWithCatalog = this.valueSeletecd;
  }
  
  onSelectChange() {
    this.onChange.emit(this.searchWithCatalog);
  }

  searchWithOptionChange(value: string) {
    this.searchType = value;
    this.onSearchTypeChange.emit(value);
  }

  unCheckedOption(item) {
    event.stopPropagation();
    if(this.searchWithCatalog.includes(item)) {
      this.searchWithCatalog = this.searchWithCatalog.filter(el => el !== item)
    }
    this.onChange.emit(this.searchWithCatalog);
  }

  unCheckedAll() {
    this.searchWithCatalog = [];
    this.onChange.emit(this.searchWithCatalog);
  }

  getOptionDisplayName(optionValue) {
    return this.catalogConfig?.catalogList.find(item => item[this.catalogConfig.optionValue] === optionValue)[this.catalogConfig.optionLabel];
  }

  onPanelShow() {
    if(this.appendTo) {
      const multiSelectPosition: DOMRect = this.multiSelectRadio.el.nativeElement.getBoundingClientRect();
      this.multiSelectRadio.overlay.style.maxWidth = this.multiSelectRadio.overlay.style.minWidth;
      if(multiSelectPosition?.left) this.multiSelectRadio.overlay.style.left = `${multiSelectPosition?.left}px`;
    }
  }
}

