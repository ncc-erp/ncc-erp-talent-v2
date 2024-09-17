import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'talent-radio-switch',
  templateUrl: './talent-radio-switch.component.html',
  styleUrls: ['./talent-radio-switch.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RadioSwitchComponent),
      multi: true
    }
  ]
})
export class RadioSwitchComponent implements ControlValueAccessor {
  @Input() inputData: {label: string, value: any}[];
  @Input() switchLabel: string;

  public _value;
  public onChange: (value: string) => void;
  public isDisabled: boolean = false;

  constructor() { }

  registerOnTouched(fn: any): void {}

  writeValue(value: any): void {
    this._value = value;
  }

  registerOnChange(fn: any): void {
    this.onChange = fn;
  }

  setDisabledState?(isDisabled: boolean): void {
    this.isDisabled = isDisabled;
  }

  ngModelChange(value) {
    this.writeValue(value);
    this.onChange(value);
  }
}
