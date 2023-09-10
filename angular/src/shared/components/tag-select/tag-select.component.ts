import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'talent-tag-select',
  templateUrl: './tag-select.component.html',
  styleUrls: ['./tag-select.component.scss']
})
export class TagSelectComponent implements OnInit {

  @Input() currentItem;
  @Input() seletedIds: number[] = [];
  @Input() nameId = 'id';

  @Output() onTagSelected = new EventEmitter<any>();

  constructor() { }

  ngOnInit(): void {
  }

  isSelectedAlready(): boolean {
    return this.seletedIds.includes(this.currentItem[this.nameId]);
  }

  seletedItem() {
    if (!this.seletedIds.includes(this.currentItem[this.nameId])) {
      this.seletedIds.push(this.currentItem[this.nameId]);
      this.onTagSelected.emit(this.currentItem);
    }
  }

}
