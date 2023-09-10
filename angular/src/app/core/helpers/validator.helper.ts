import { AbstractControl, ValidatorFn } from '@angular/forms';
import { DateFormat } from '@shared/AppConsts';
import * as moment from 'moment';

export class CustomValidators {
 
  static isToDateMustGreaterThanOrEqual(fromDateField: string, toDateField: string): ValidatorFn {
    return (formGroup: AbstractControl): { [key: string]: boolean } | null => {
      const fromDate = moment(formGroup.get(fromDateField).value, DateFormat.YYYY_MM_DD);
      const toDate = moment(formGroup.get(toDateField).value, DateFormat.YYYY_MM_DD);

      const isAfter = toDate.isSameOrAfter(fromDate);
      if (!isAfter) {
        return { greaterThan: true };
      }
      return null;
    };
  }

  static isDateMustLessThanCurrent(): ValidatorFn {
    return (formControl: AbstractControl) => {
      const dateInput = moment(formControl.value, DateFormat.YYYY_MM_DD)
      const currentDate = moment(new Date(), DateFormat.YYYY_MM_DD);

      const isAfter = dateInput.isSameOrAfter(currentDate);
      if (isAfter) {
        return { mustLessThanCurrentDate: true };
      }
      return null;
    };
  }

  static isDateMustGreaterThanCurrent(): ValidatorFn {
    return (formControl: AbstractControl) => {
      const dateInput = moment(formControl.value, DateFormat.YYYY_MM_DD);
      const currentDate = moment(new Date(), DateFormat.YYYY_MM_DD);

      const isBefore = dateInput.isBefore(currentDate);
      if (isBefore) {
        return { mustGreaterThanCurrentDate: true };
      }
      return null;
    };
  }
  
}