import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'talent-position-dropdown',
  templateUrl: './position-dropdown.component.html',
  styleUrls: ['./position-dropdown.component.scss']
})
export class PositionDropdownComponent implements OnInit {


  @Input() id: string;
  @Input() options: [];
  @Input() optionSelected: string = null;
  @Input() disabled = false;
  @Output() onChange = new EventEmitter<any>();

  constructor() { }

  ngOnInit(): void { }

  onSelectChange(value: number) {
    this.onChange.emit(value);
  }

}
